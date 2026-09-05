using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using LeaveManagement.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Handler.Queries.GetAllLeaveBalances
{
    public class GetAllLeaveBalancesHandler : IQueryHandler<GetAllLeaveBalancesQuery, HandlerResult<IEnumerable<LeaveBalanceResponse>>>
    {
        private readonly LeaveDbContext _dbContext;

        public GetAllLeaveBalancesHandler(LeaveDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<IEnumerable<LeaveBalanceResponse>>> HandleAsync(GetAllLeaveBalancesQuery query)
        {
            var balances = await _dbContext.LeaveBalances
                .Include(b => b.LeaveType)
                .Where(b => b.EmployeeId == query.EmployeeId && b.Year == query.Year)
                .Select(b => new LeaveBalanceResponse
                {
                    LeaveBalanceId = b.LeaveBalanceId,
                    EmployeeId = b.EmployeeId,
                    LeaveTypeId = b.LeaveTypeId,
                    LeaveTypeName = b.LeaveType != null ? b.LeaveType.Name : string.Empty,
                    Year = b.Year,
                    TotalDays = b.TotalDays,
                    UsedDays = b.UsedDays,
                    HeldDays = b.HeldDays,
                    AvailableDays = b.TotalDays - b.UsedDays - b.HeldDays,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();

            return HandlerResult<IEnumerable<LeaveBalanceResponse>>.SuccessResult(balances);
        }
    }
}
