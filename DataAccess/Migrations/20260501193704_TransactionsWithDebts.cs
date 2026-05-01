using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TransactionsWithDebts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DebtId",
                table: "Transactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "DebtId",
                table: "PlannedTransactions",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_DebtId",
                table: "Transactions",
                column: "DebtId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedTransactions_DebtId",
                table: "PlannedTransactions",
                column: "DebtId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedTransactions_Debts_DebtId",
                table: "PlannedTransactions",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Debts_DebtId",
                table: "Transactions",
                column: "DebtId",
                principalTable: "Debts",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlannedTransactions_Debts_DebtId",
                table: "PlannedTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Debts_DebtId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_DebtId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_PlannedTransactions_DebtId",
                table: "PlannedTransactions");

            migrationBuilder.DropColumn(
                name: "DebtId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DebtId",
                table: "PlannedTransactions");
        }
    }
}
