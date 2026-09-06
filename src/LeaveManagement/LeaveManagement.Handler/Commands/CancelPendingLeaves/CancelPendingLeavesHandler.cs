using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Exceptions;
using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Response;
using LeaveManagement.Repository.Interfaces;
using LeaveManagement.Aggregator.Exceptions;
using LeaveManagement.Aggregator.Entities;

namespace LeaveManagement.Handler.Commands.CancelPendingLeaves
{
    public class CancelPendingLeavesHandler : ICommandHandler<CancelPendingLeavesCommand, HandlerResult<CancelPendingLeavesResponse>>
    {
        private readonly ILeaveRequestRepository _requestRepository;
        private readonly ILeaveBalanceRepository _balanceRepository;
        private readonly ILeaveUnitOfWork _unitOfWork;

        public CancelPendingLeavesHandler(
            ILeaveRequestRepository requestRepository,
            ILeaveBalanceRepository balanceRepository,
            ILeaveUnitOfWork unitOfWork)
        {
            _requestRepository = requestRepository;
            _balanceRepository = balanceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<HandlerResult<CancelPendingLeavesResponse>> HandleAsync(CancelPendingLeavesCommand command)
        {
            // Get all pending requests for the employee
            var pagedResult = await _requestRepository.GetPagedAsync(command.EmployeeId, null, "Pending", 1, int.MaxValue);
            var pendingRequests = pagedResult.Requests;

            if (!pendingRequests.Any())
            {
                return HandlerResult<CancelPendingLeavesResponse>.SuccessResult(
                    new CancelPendingLeavesResponse { CancelledCount = 0 }, 
                    "No pending leave requests found."
                );
            }

            // We need to fetch balances first to apply domain logic
            var balanceDict = new Dictionary<int, LeaveBalance>();
            foreach (var request in pendingRequests)
            {
                int year = request.StartDate.Year;
                var balance = await _balanceRepository.GetByEmployeeAndTypeAsync(request.EmployeeId, request.LeaveTypeId, year);
                if (balance != null)
                {
                    balanceDict[request.LeaveRequestId] = balance;
                }
            }

            foreach (var request in pendingRequests)
            {
                if (!balanceDict.TryGetValue(request.LeaveRequestId, out var balance))
                    continue;

                try
                {
                    // Release the held days
                    balance.ReleaseHold(request.TotalDays);
                    
                    // Cancel the request
                    request.Cancel();
                }
                catch (DomainException ex)
                {
                    return HandlerResult<CancelPendingLeavesResponse>.FailureResult(ex.Message);
                }
            }

            try
            {
                await _unitOfWork.ExecuteInTransactionAsync(async () =>
                {
                    foreach (var request in pendingRequests)
                    {
                        if (balanceDict.TryGetValue(request.LeaveRequestId, out var balance))
                        {
                            await _requestRepository.UpdateAsync(request);
                            await _balanceRepository.UpdateAsync(balance);
                        }
                    }
                });
            }
            catch (ConcurrencyException)
            {
                return HandlerResult<CancelPendingLeavesResponse>.FailureResult("A concurrency error occurred while updating the leave balances. Please try again.");
            }

            return HandlerResult<CancelPendingLeavesResponse>.SuccessResult(
                new CancelPendingLeavesResponse { CancelledCount = pendingRequests.Count },
                $"Successfully cancelled {pendingRequests.Count} pending leave requests."
            );
        }
    }
}

