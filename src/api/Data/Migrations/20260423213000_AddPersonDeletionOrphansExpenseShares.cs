using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.src.api.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonDeletionOrphansExpenseShares : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseShares_People_PersonId",
                table: "ExpenseShares");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseShares_People_PersonId",
                table: "ExpenseShares",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpenseShares_People_PersonId",
                table: "ExpenseShares");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpenseShares_People_PersonId",
                table: "ExpenseShares",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
