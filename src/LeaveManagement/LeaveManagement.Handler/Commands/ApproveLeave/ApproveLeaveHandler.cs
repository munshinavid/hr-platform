using System;
using System.Threading.Tasks;
using LeaveManagement.Aggregator.Exceptions;
using LeaveManagement.DTO.Command;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Exceptions;

namespace LeaveManagement.Handler.Commands.ApproveLeave
{
    public class ApproveLeaveHandler : ICommandHandler<ApproveLeaveCommand, HandlerResult>
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly ILeaveUnitOfWork _unitOfWork;

        public ApproveLeaveHandler(
            ILeaveRequestRepository requestRepository,
            ILeaveBalanceRepository balanceRepository,
            ILeaveUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _balanceRepository = balanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<HandlerResult> HandleAsync(ApproveLeaveCommand command)
        {
            var request = await _requestRepository.GetByIdAsync(command.LeaveRequestId);
            if (request == null)
                return HandlerResult.FailureResult("Leave request not found.");

            int year = request.StartDate.Year;
            var balance = await _balanceRepository.GetByEmployeeAndTypeAsync(request.EmployeeId, request.LeaveTypeId, year);

            if (balance == null)
                return HandlerResult.FailureResult("Leave balance not found.");

            try
            {
                request.Approve(command.ApprovedByEmployeeId);
                balance.UseHold(request.TotalDays);
            }
            catch (DomainException ex)
            {
                return HandlerResult.FailureResult(ex.Message);
            }

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await _requestRepository.UpdateAsync(request);
                    await _balanceRepository.UpdateAsync(balance);
                });
            }
            catch (ConcurrencyException)
            {
                return HandlerResult.FailureResult("A concurrency error occurred while updating the leave balance. Please try again.");
            }

            return HandlerResult.SuccessResult("Leave approved successfully.");
        }
    }
}
