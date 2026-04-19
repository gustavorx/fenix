using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.src.api.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseShareInstallments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExpenseShareInstallments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PaidDate = table.Column<DateOnly>(type: "date", nullable: true),
                    ExpenseShareId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpenseShareInstallments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseShareInstallments_ExpenseShares_ExpenseShareId",
                        column: x => x.ExpenseShareId,
                        principalTable: "ExpenseShares",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseShareInstallments_ExpenseShareId",
                table: "ExpenseShareInstallments",
                column: "ExpenseShareId");

            migrationBuilder.Sql("""
                INSERT INTO "ExpenseShareInstallments" ("Id", "Amount", "DueDate", "PaidDate", "ExpenseShareId")
                SELECT
                    gen_random_uuid(),
                    es."Amount",
                    COALESCE(
                        (
                            SELECT MIN(i."DueDate")
                            FROM "Installments" AS i
                            WHERE i."ExpenseId" = es."ExpenseId"
                        ),
                        e."PurchaseDate"
                    ),
                    CASE
                        WHEN es."Paid" THEN COALESCE(
                            es."PaymentDate"::date,
                            (
                                SELECT MIN(i."DueDate")
                                FROM "Installments" AS i
                                WHERE i."ExpenseId" = es."ExpenseId"
                            ),
                            e."PurchaseDate"
                        )
                        ELSE NULL
                    END,
                    es."Id"
                FROM "ExpenseShares" AS es
                INNER JOIN "Expenses" AS e ON e."Id" = es."ExpenseId";
                """);

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "ExpenseShares");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "ExpenseShares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Paid",
                table: "ExpenseShares",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "ExpenseShares",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "ExpenseShares" AS es
                SET "Paid" = summary."IsFullyPaid",
                    "PaymentDate" = summary."PaymentDate"
                FROM (
                    SELECT
                        i."ExpenseShareId",
                        bool_and(i."PaidDate" IS NOT NULL) AS "IsFullyPaid",
                        CASE
                            WHEN bool_and(i."PaidDate" IS NOT NULL)
                                THEN MAX(i."PaidDate")::timestamp with time zone
                            ELSE NULL::timestamp with time zone
                        END AS "PaymentDate"
                    FROM "ExpenseShareInstallments" AS i
                    GROUP BY i."ExpenseShareId"
                ) AS summary
                WHERE es."Id" = summary."ExpenseShareId";
                """);

            migrationBuilder.DropTable(
                name: "ExpenseShareInstallments");
        }
    }
}
