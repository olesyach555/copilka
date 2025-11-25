using System;
using Microsoft.EntityFrameworkCore.Migrations;
using BCrypt.Net;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToTransactionAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // --- Добавление столбца Type ---
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // --- Добавление столбца UserId ---
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

            // --- Создание пользователя ---
            var userId = 2;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password123");
            migrationBuilder.Sql(@$"INSERT INTO Users (Id, Login, PasswordHash, Role, Email) VALUES ({userId}, 'testuser', '{hashedPassword}', 'User', 'testuser@example.com');");

            // --- Создание счетов ---
            migrationBuilder.Sql(@$"INSERT INTO Accounts (Id, UserId, Name, Type, Currency, Balance) VALUES (4, {userId}, 'Карта Сбербанк', 'Card', 'RUB', 10000);");
            migrationBuilder.Sql(@$"INSERT INTO Accounts (Id, UserId, Name, Type, Currency, Balance) VALUES (5, {userId}, 'Наличные', 'Cash', 'RUB', 5000);");

            // --- Создание категорий ---
            migrationBuilder.Sql(@$"INSERT INTO Categories (Id, UserId, Name, Type, Description) VALUES (8, {userId}, 'Зарплата', 'Income', 'Основной доход');");
            migrationBuilder.Sql(@$"INSERT INTO Categories (Id, UserId, Name, Type, Description) VALUES (9, {userId}, 'Кофе', 'Expense', 'Расходы на кофе');");
            migrationBuilder.Sql(@$"INSERT INTO Categories (Id, UserId, Name, Type, Description) VALUES (10, {userId}, 'Такси', 'Expense', 'Расходы на такси');");
            migrationBuilder.Sql(@$"INSERT INTO Categories (Id, UserId, Name, Type, Description) VALUES (11, {userId}, 'Продукты', 'Expense', 'Расходы на продукты');");

            // --- Создание транзакций ---
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (7, {userId}, 4, 8, 'Income', 50000, '{DateTime.UtcNow.AddDays(-10):yyyy-MM-dd HH:mm:ss}', 'Аванс');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (8, {userId}, 4, 9, 'Expense', 350, '{DateTime.UtcNow.AddDays(-5):yyyy-MM-dd HH:mm:ss}', 'Кофе в Starbucks');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (9, {userId}, 5, 10, 'Expense', 500, '{DateTime.UtcNow.AddDays(-2):yyyy-MM-dd HH:mm:ss}', 'Поездка на работу');");
            migrationBuilder.Sql(@$"INSERT INTO Transactions (Id, UserId, AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (10, {userId}, 4, 11, 'Expense', 2500, '{DateTime.UtcNow.AddDays(-1):yyyy-MM-dd HH:mm:ss}', 'Закупка в Перекрестке');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM Transactions WHERE UserId = 2;");
            migrationBuilder.Sql(@"DELETE FROM Categories WHERE UserId = 2;");
            migrationBuilder.Sql(@"DELETE FROM Accounts WHERE UserId = 2;");
            migrationBuilder.Sql(@"DELETE FROM Users WHERE Login = 'testuser';");

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
