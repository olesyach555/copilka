using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreateForSqlServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OperationType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_CategoryId",
                table: "Transactions",
                column: "CategoryId");

            // --- НАЧАЛО ЗАПОЛНЕНИЯ ДАННЫМИ ---

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
                    { 4, 1, "Коммунальные платежи", "Expense", "" },
                    { 8, 1, "Развлечения", "Expense", "" } // Новая категория
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
                columns: new[] { "Id", "AccountId", "CategoryId", "Amount", "Date", "Comment", "Type" },
                values: new object[,]
                {
                    { 1, 1, 2, 5014.90m, new DateTime(2025, 11, 1), "Кофе", "Expense" },
                    { 2, 2, 3, 890.00m, new DateTime(2025, 11, 2), "Такси", "Expense" },
                    { 3, 3, 4, 103400m, new DateTime(2025, 11, 3), "Коммунальные платежи", "Expense" },
                    { 4, 1, 1, 1200.50m, new DateTime(2025, 11, 5), "Продукты", "Expense" },
                    { 5, 2, 5, 3500.00m, new DateTime(2025, 11, 10), "Зарплата (доход)", "Income" },
                    { 6, 1, 8, 450.00m, new DateTime(2025, 11, 15), "Развлечения", "Expense" },
                    { 7, 3, 3, 700.00m, new DateTime(2025, 11, 20), "Такси", "Expense" },
                    { 8, 2, 2, 950.00m, new DateTime(2025, 11, 21), "Кофе", "Expense" }
                });

            // --- КОНЕЦ ЗАПОЛНЕНИЯ ДАННЫМИ ---
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Dates");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
