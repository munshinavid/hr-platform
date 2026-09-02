using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using LeaveManagement.Repository;
using HRPlatform.Shared.Abstractions;
using HRPlatform.Shared.Common;
using HRPlatform.Shared.Dispatcher;
using LeaveManagement.DTO.Command;
using LeaveManagement.DTO.Query;
using LeaveManagement.DTO.Response;
using LeaveManagement.Handler.Commands.ApplyLeave;
using LeaveManagement.Handler.Commands.ApproveLeave;
using LeaveManagement.Handler.Commands.RejectLeave;
using LeaveManagement.Handler.Commands.CancelLeave;
using LeaveManagement.Handler.Queries.GetLeaveBalance;
using LeaveManagement.Handler.Queries.GetLeaveRequest;
using LeaveManagement.Handler.Queries.GetLeaveRequests;

namespace LeaveManagement.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLeaveHandlerLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRepositoryLayer(configuration);

            // Commands
            services.AddScoped<ICommandHandler<ApplyLeaveCommand, HandlerResult<LeaveRequestResponse>>, ApplyLeaveHandler>();
            services.AddScoped<ICommandHandler<ApproveLeaveCommand, HandlerResult>, ApproveLeaveHandler>();
            services.AddScoped<ICommandHandler<RejectLeaveCommand, HandlerResult>, RejectLeaveHandler>();
            services.AddScoped<ICommandHandler<CancelLeaveCommand, HandlerResult>, CancelLeaveHandler>();
            services.AddScoped<ICommandHandler<CancelPendingLeavesCommand, HandlerResult<CancelPendingLeavesResponse>>, Commands.CancelPendingLeaves.CancelPendingLeavesHandler>();

            // Queries
            services.AddScoped<IQueryHandler<GetLeaveBalanceQuery, HandlerResult<LeaveBalanceResponse>>, GetLeaveBalanceHandler>();
            services.AddScoped<IQueryHandler<GetAllLeaveBalancesQuery, HandlerResult<IEnumerable<LeaveBalanceResponse>>>, Queries.GetAllLeaveBalances.GetAllLeaveBalancesHandler>();
            services.AddScoped<IQueryHandler<GetLeaveRequestQuery, HandlerResult<LeaveRequestResponse>>, GetLeaveRequestHandler>();
            services.AddScoped<IQueryHandler<GetLeaveRequestsQuery, HandlerResult<PagedResponse<LeaveRequestResponse>>>, GetLeaveRequestsHandler>();

            // Dispatcher
            services.AddScoped<IDispatcher, Dispatcher>();

            return services;
        }
    }
}
