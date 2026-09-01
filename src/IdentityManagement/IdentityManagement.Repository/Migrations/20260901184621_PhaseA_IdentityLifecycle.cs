using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IdentityManagement.Repository.Migrations
{
    /// <inheritdoc />
    public partial class PhaseA_IdentityLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── Safety note ───────────────────────────────────────────────────────
            // The Users table already exists in the database (created before EF
            // migrations were introduced for this context).
            // This migration only ADDS and DROPS columns — it never drops or recreates
            // the table, so existing UserId / Email / Password / Role data is preserved.
            // ─────────────────────────────────────────────────────────────────────

            // 1. Add IsActive — nullable first so SQL Server can fill existing rows.
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: true);

            // 2. Backfill: all existing users are treated as active (safe default).
            migrationBuilder.Sql("UPDATE [Users] SET [IsActive] = 1 WHERE [IsActive] IS NULL");

            // 3. Now tighten to NOT NULL with a column default for future inserts.
            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            // 4. Add CreatedAt — nullable first, backfill with a sentinel UTC timestamp.
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Users] SET [CreatedAt] = '2026-01-01T00:00:00' WHERE [CreatedAt] IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            // 5. Add UpdatedAt — same backfill pattern.
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.Sql("UPDATE [Users] SET [UpdatedAt] = '2026-01-01T00:00:00' WHERE [UpdatedAt] IS NULL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            // 6. Drop Name — identity credentials do not own the HR display name.
            //    Employee.Name in EmployeeManagement is the authoritative HR profile name.
            //    ONLY run this if the column exists; if it was never added, this is a no-op.
            migrationBuilder.Sql(@"
                IF COL_LENGTH('Users', 'Name') IS NOT NULL
                    ALTER TABLE [Users] DROP COLUMN [Name]");

            // 7. Unique index on Email for fast login lookup.
            //    IF NOT EXISTS guard makes this safe to run on databases that already have it.
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
                    CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email])");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the unique index.
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('Users'))
                    DROP INDEX [IX_Users_Email] ON [Users]");

            // Re-add Name column (restored as nullable for safety).
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: string.Empty);

            // Drop the lifecycle and audit columns.
            migrationBuilder.DropColumn(name: "UpdatedAt", table: "Users");
            migrationBuilder.DropColumn(name: "CreatedAt", table: "Users");
            migrationBuilder.DropColumn(name: "IsActive", table: "Users");
        }
    }
}
