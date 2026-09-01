using System;
using System.Threading.Tasks;
using LeaveManagement.Aggregator.Exceptions;
using LeaveManagement.DTO.Command;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using Microsoft.EntityFrameworkCore;
using LeaveManagement.Repository.Data;

namespace LeaveManagement.Handler.Commands.RejectLeave
{
    public class RejectLeaveHandler : ICommandHandler<RejectLeaveCommand, HandlerResult>
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly LeaveDbContext _dbContext;

        public RejectLeaveHandler(
            ILeaveRequestRepository requestRepository,
            ILeaveBalanceRepository balanceRepository,
            LeaveDbContext dbContext)
        {
            _requestRepository = requestRepository;
            _balanceRepository = balanceRepository;
            _dbContext = dbContext;
        }

        public async Task<HandlerResult> HandleAsync(RejectLeaveCommand command)
        {
            try
            {
                var request = await _requestRepository.GetByIdAsync(command.LeaveRequestId);
                if (request == null)
                    return HandlerResult.FailureResult("Leave request not found.");

                int year = request.StartDate.Year;
                var balance = await _balanceRepository.GetByEmployeeAndTypeAsync(request.EmployeeId, request.LeaveTypeId, year);

                if (balance == null)
                    return HandlerResult.FailureResult("Leave balance not found.");

                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                request.Reject(command.RejectedByEmployeeId, command.RejectionReason);
                balance.ReleaseHold(request.TotalDays);

                await _requestRepository.UpdateAsync(request);
                await _balanceRepository.UpdateAsync(balance);

                await transaction.CommitAsync();

                return HandlerResult.SuccessResult("Leave rejected successfully.");
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                return HandlerResult.FailureResult("A concurrency error occurred while updating the leave balance. Please try again.");
            }
            catch (Exception ex)
            {
                return HandlerResult.FailureResult($"An error occurred: {ex.Message}");
            }
        }
    }
}
