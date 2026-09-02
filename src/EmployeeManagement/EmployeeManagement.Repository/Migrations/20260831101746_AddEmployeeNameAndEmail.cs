using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeeNameAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false),
                    JobTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salary = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    EmploymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JoiningDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_Employees_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "DepartmentId", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "IT" },
                    { 2, "HR" },
                    { 3, "Finance" }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "DepartmentId", "Email", "EmploymentType", "Gender", "JobTitle", "JoiningDate", "Name", "Phone", "Salary", "Status", "UserId" },
                values: new object[,]
                {
                    { 1, 1, "", "Full-Time", "Male", "Software Engineer", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "01711111111", 45000m, "Active", 3 },
                    { 2, 2, "", "Full-Time", "Male", "HR Executive", new DateTime(2024, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "01722222222", 35000m, "Active", 4 },
                    { 3, 3, "", "Full-Time", "Female", "Accountant", new DateTime(2025, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "01733333333", 40000m, "Active", 5 },
                    { 4, 1, "", "Contract", "Male", "Web Developer", new DateTime(2023, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "01744444444", 50000m, "Inactive", 6 },
                    { 5, 2, "", "Full-Time", "Female", "HR Manager", new DateTime(2022, 11, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "01755555555", 60000m, "Active", 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_DepartmentId",
                table: "Employees",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
