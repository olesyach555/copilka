using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedApplicationData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Хэшируем пароль для тестового пользователя
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");

            // Добавляем тестового пользователя
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Login", "PasswordHash", "Email", "Role" },
                values: new object[] { 1, "testuser", passwordHash, "testuser@example.com", "User" });

            // Добавляем тестовые счета
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "UserId", "Name", "Balance", "Type", "Currency" },
                values: new object[,]
                {
                    { 1, 1, "Альфа-Банк", 0.00m, "Карта", "RUB" },
                    { 2, 1, "Сбербанк", 0.00m, "Карта", "RUB" },
                    { 3, 1, "Сберегательный счет", 0.00m, "Сберегательный", "RUB" }
                });

            // Добавляем тестовые категории расходов
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "UserId", "Name", "Type", "Description" },
                values: new object[,]
                {
                    { 1, 1, "Продукты", "Expense", "" },
                    { 2, 1, "Кофе", "Expense", "" },
                    { 3, 1, "Такси", "Expense", "" },
                    { 4, 1, "Коммунальные платежи", "Expense", "" }
                });

            // Добавляем тестовые категории доходов
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "UserId", "Name", "Type", "Description" },
                values: new object[,]
                {
                    { 5, 1, "Зарплата", "Income", "" },
                    { 6, 1, "Премия", "Income", "" },
                    { 7, 1, "Доход от фриланса", "Income", "" }
                });

            // Добавляем тестовые транзакции
            migrationBuilder.InsertData(
                table: "Transactions",
                columns: new[] { "Id", "AccountId", "CategoryId", "Amount", "Date", "Comment" },
                values: new object[,]
                {
                    // Расходы
                    { 1, 1, 1, 1500.50m, new DateTime(2023, 1, 15), "Покупка в супермаркете" },
                    { 2, 1, 2, 350.00m, new DateTime(2023, 1, 16), "Кофе с собой" },
                    { 3, 2, 3, 890.00m, new DateTime(2023, 1, 17), "Поездка на работу" },
                    { 4, 2, 4, 5500.00m, new DateTime(2023, 1, 20), "Оплата коммунальных услуг" },
                    // Доходы
                    { 5, 1, 5, 70000.00m, new DateTime(2023, 1, 10), "Зарплата за январь" },
                    { 6, 3, 6, 15000.00m, new DateTime(2023, 1, 25), "Годовая премия" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Удаляем транзакции
            migrationBuilder.DeleteData(table: "Transactions", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4, 5, 6 });

            // Удаляем категории
            migrationBuilder.DeleteData(table: "Categories", keyColumn: "Id", keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7 });

            // Удаляем счета
            migrationBuilder.DeleteData(table: "Accounts", keyColumn: "Id", keyValues: new object[] { 1, 2, 3 });

            // Удаляем пользователя
            migrationBuilder.DeleteData(table: "Users", keyColumn: "Id", keyValue: 1);
        }
    }
}
