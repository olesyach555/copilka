using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddAllTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Шаг 1: Изменение схемы ---
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Transactions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // --- Шаг 2: Добавление запрошенных данных ---
            // Привязываем все транзакции к UserId=1 (testuser) и AccountId=1 (Альфа-Банк)

            // Расходы
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (7, 1, 1, 1, 'Expense', 850.00, '2025-11-26 14:30:00', 'Покупка продуктов в Магните');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (8, 1, 1, 2, 'Expense', 450.00, '2025-11-25 09:15:00', 'Утренний кофе в Coffee House');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (9, 1, 1, 3, 'Expense', 650.00, '2025-11-24 19:45:00', 'Поездка домой после работы');");

            // Доходы
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (10, 1, 1, 5, 'Income', 35000.00, '2025-11-25 10:00:00', 'Зарплата за ноябрь');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (11, 1, 1, 6, 'Income', 5000.00, '2025-11-20 16:20:00', 'Премия за выполнение проекта');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // --- Шаг 1: Удаление данных ---
            migrationBuilder.Sql(@"DELETE FROM Transactions WHERE Id IN (7, 8, 9, 10, 11);");

            // --- Шаг 2: Откат схемы ---
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Users_UserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");
        }
    }
}
