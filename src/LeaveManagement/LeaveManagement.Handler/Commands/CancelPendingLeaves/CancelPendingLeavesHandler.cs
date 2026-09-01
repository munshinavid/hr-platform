using System;
using System.Linq;
using System.Threading.Tasks;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Response;
using LeaveManagement.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Handler.Commands.CancelPendingLeaves
{
    public class CancelPendingLeavesHandler : ICommandHandler<CancelPendingLeavesCommand, HandlerResult<CancelPendingLeavesResponse>>
    {
        private readonly LeaveDbContext _dbContext;

        public CancelPendingLeavesHandler(LeaveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<CancelPendingLeavesResponse>> HandleAsync(CancelPendingLeavesCommand command)
        {
            try
            {
                using var transaction = await _dbContext.Database.BeginTransactionAsync();

                // Get all pending requests for the employee
                var pendingRequests = await _dbContext.LeaveRequests
                    .Where(r => r.EmployeeId == command.EmployeeId && r.Status == "Pending")
                    .ToListAsync();

                if (!pendingRequests.Any())
                {
                    return HandlerResult<CancelPendingLeavesResponse>.SuccessResult(
                        new CancelPendingLeavesResponse { CancelledCount = 0 }, 
                        "No pending leave requests found."
                    );
                }

                foreach (var request in pendingRequests)
                {
                    // Find the corresponding balance
                    int year = request.StartDate.Year;
                    var balance = await _dbContext.LeaveBalances
                        .FirstOrDefaultAsync(b => b.EmployeeId == request.EmployeeId && b.LeaveTypeId == request.LeaveTypeId && b.Year == year);

                    if (balance == null)
                        continue;

                    // Release the held days
                    balance.ReleaseHold(request.TotalDays);
                    
                    // Cancel the request
                    request.Cancel();
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return HandlerResult<CancelPendingLeavesResponse>.SuccessResult(
                    new CancelPendingLeavesResponse { CancelledCount = pendingRequests.Count },
                    $"Successfully cancelled {pendingRequests.Count} pending leave requests."
                );
            }
            catch (DbUpdateConcurrencyException)
            {
                return HandlerResult<CancelPendingLeavesResponse>.FailureResult("A concurrency error occurred while updating the leave balances. Please try again.");
            }
            catch (Exception ex)
            {
                return HandlerResult<CancelPendingLeavesResponse>.FailureResult($"An error occurred: {ex.Message}");
            }
        }
    }
}
