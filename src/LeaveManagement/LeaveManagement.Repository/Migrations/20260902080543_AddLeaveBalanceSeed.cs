using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LeaveManagement.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddLeaveBalanceSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "LeaveBalances",
                columns: new[] { "LeaveBalanceId", "CreatedAt", "EmployeeId", "HeldDays", "LeaveTypeId", "TotalDays", "UpdatedAt", "UsedDays", "Year" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, 1, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 2026 },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, 2, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 2026 },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1, 0, 3, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 2026 },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 0, 1, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 2, 2026 },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 5, 0, 1, 20, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0, 2026 }
                });

            migrationBuilder.InsertData(
                table: "LeaveRequests",
                columns: new[] { "LeaveRequestId", "ApprovedByEmployeeId", "CreatedAt", "EmployeeId", "EndDate", "LeaveTypeId", "Reason", "RejectionReason", "RequestedAt", "ReviewedAt", "StartDate", "Status", "TotalDays", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Family vacation", null, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Pending", 3, new DateTime(2026, 9, 1, 10, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 5, new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), 2, new DateTime(2026, 8, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Personal work", null, new DateTime(2026, 8, 1, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 2, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 8, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approved", 2, new DateTime(2026, 8, 2, 14, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 5, new DateTime(2026, 6, 25, 11, 0, 0, 0, DateTimeKind.Utc), 1, new DateTime(2026, 7, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Urgent travel", "Critical release scheduled on those dates", new DateTime(2026, 6, 25, 11, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 6, 26, 16, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Rejected", 2, new DateTime(2026, 6, 26, 16, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "LeaveBalances",
                keyColumn: "LeaveBalanceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveBalances",
                keyColumn: "LeaveBalanceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveBalances",
                keyColumn: "LeaveBalanceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "LeaveBalances",
                keyColumn: "LeaveBalanceId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "LeaveBalances",
                keyColumn: "LeaveBalanceId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "LeaveRequestId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "LeaveRequestId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "LeaveRequests",
                keyColumn: "LeaveRequestId",
                keyValue: 3);
        }
    }
}
