using System;
using System.Threading.Tasks;
using LeaveManagement.Aggregator.Entities;
using LeaveManagement.Aggregator.Exceptions;
using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Response;
using LeaveManagement.Aggregator.Mapping;
using LeaveManagement.Repository.Interfaces;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Exceptions;

namespace LeaveManagement.Handler.Commands.ApplyLeave
{
    public class ApplyLeaveHandler : ICommandHandler<ApplyLeaveCommand, HandlerResult<LeaveRequestResponse>>
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly IGenericRepository<LeaveType> _typeRepository;
        private readonly ILeaveUnitOfWork _unitOfWork;

        public ApplyLeaveHandler(
            ILeaveRequestRepository requestRepository,
            ILeaveBalanceRepository balanceRepository,
            IGenericRepository<LeaveType> typeRepository,
            ILeaveUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _balanceRepository = balanceRepository;
            _typeRepository = typeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<HandlerResult<LeaveRequestResponse>> HandleAsync(ApplyLeaveCommand command)
        {
            var leaveType = await _typeRepository.GetByIdAsync(command.LeaveTypeId);
            if (leaveType == null || !leaveType.IsActive)
                return HandlerResult<LeaveRequestResponse>.FailureResult(
                    Error.Validation("INVALID_LEAVE_TYPE", "Invalid or inactive Leave Type."));

            int year = command.StartDate.Year;
            var balance = await _balanceRepository.GetByEmployeeAndTypeAsync(command.EmployeeId, command.LeaveTypeId, year);
            
            if (balance == null)
                return HandlerResult<LeaveRequestResponse>.FailureResult(
                    Error.NotFound("LEAVE_BALANCE_NOT_FOUND", $"No leave balance found for year {year}."));

            int totalDays = (command.EndDate - command.StartDate).Days + 1;

            LeaveRequest request;
            
            try
            {
                request = LeaveRequest.Apply(
                    command.EmployeeId,
                    command.LeaveTypeId,
                    command.StartDate,
                    command.EndDate,
                    totalDays,
                    command.Reason);

                balance.Hold(totalDays);
            }
            catch (DomainException ex)
            {
                return HandlerResult<LeaveRequestResponse>.FailureResult(
                    Error.Validation("DOMAIN_RULE_VIOLATION", ex.Message));
            }

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    await _requestRepository.AddAsync(request);
                    await _balanceRepository.UpdateAsync(balance);
                });
            }
            catch (ConcurrencyException)
            {
                return HandlerResult<LeaveRequestResponse>.FailureResult(
                    Error.Conflict("CONCURRENCY_ERROR", "A concurrency error occurred while updating the leave balance. Please try again."));
            }

            var response = LeaveMapper.MapToResponse(request);
            return HandlerResult<LeaveRequestResponse>.SuccessResult(response, "Leave applied successfully.");
        }
    }
}
