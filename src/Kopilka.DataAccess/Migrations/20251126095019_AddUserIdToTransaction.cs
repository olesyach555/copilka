using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add the column as nullable first to avoid constraint violations on existing data.
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            // Populate the new UserId column for existing transactions from the associated account.
            migrationBuilder.Sql(
                "UPDATE \"Transactions\" SET \"UserId\" = (SELECT a.\"UserId\" FROM \"Accounts\" a WHERE a.\"Id\" = \"Transactions\".\"AccountId\")");

            // Now that all rows have a value, alter the column to be non-nullable.
            // EF Core's SQLite provider handles this by recreating the table.
            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            // Create an index on the new column for performance.
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            // Add the foreign key constraint.
            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");
        }
    }
}
