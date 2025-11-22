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

            migrationBuilder.Sql(@"
                -- Seed Users
                INSERT INTO Users (Login, PasswordHash, Role, Email) VALUES ('testuser', '$2a$11$a4.5b8d2f1f2e1f2a4b5e6f7d8c9a0b1c2d3e4f5a6b7c8d9e0f1a2b3c4d5e6f7', 'User', 'testuser@example.com');
                DECLARE @userId INT = SCOPE_IDENTITY();

                -- Seed Accounts
                INSERT INTO Accounts (UserId, Type, Balance, Name, Currency) VALUES (@userId, 'Checking', 1000.00, 'Альфа-Банк', 'RUB');
                INSERT INTO Accounts (UserId, Type, Balance, Name, Currency) VALUES (@userId, 'Savings', 5000.00, 'Сбербанк', 'RUB');
                INSERT INTO Accounts (UserId, Type, Balance, Name, Currency) VALUES (@userId, 'Savings', 10000.00, 'Сберегательный счет', 'RUB');

                -- Seed Categories
                INSERT INTO Categories (Name, Type, Description, UserId) VALUES ('Продукты', 'Expense', 'Покупка продуктов', @userId);
                INSERT INTO Categories (Name, Type, Description, UserId) VALUES ('Транспорт', 'Expense', 'Расходы на транспорт', @userId);
                INSERT INTO Categories (Name, Type, Description, UserId) VALUES ('Зарплата', 'Income', 'Получение зарплаты', @userId);

                -- Seed Transactions
                DECLARE @accountId1 INT = (SELECT Id FROM Accounts WHERE Name = 'Альфа-Банк');
                DECLARE @accountId2 INT = (SELECT Id FROM Accounts WHERE Name = 'Сбербанк');
                DECLARE @categoryId1 INT = (SELECT Id FROM Categories WHERE Name = 'Продукты');
                DECLARE @categoryId2 INT = (SELECT Id FROM Categories WHERE Name = 'Транспорт');
                DECLARE @categoryId3 INT = (SELECT Id FROM Categories WHERE Name = 'Зарплата');

                INSERT INTO Transactions (AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (@accountId1, @categoryId1, 'Expense', 150.00, GETDATE(), 'Купил молоко');
                INSERT INTO Transactions (AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (@accountId1, @categoryId2, 'Expense', 50.00, GETDATE(), 'Поездка на автобусе');
                INSERT INTO Transactions (AccountId, CategoryId, Type, Amount, Date, Comment) VALUES (@accountId2, @categoryId3, 'Income', 50000.00, GETDATE(), 'Зарплата за ноябрь');
            ");
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
