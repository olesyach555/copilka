using Microsoft.EntityFrameworkCore.Migrations;
using BCrypt.Net;

#nullable disable

namespace Kopilka.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Transactions",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            // Добавление тестового пользователя
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("TestPassword123");
            migrationBuilder.Sql(@$"
                INSERT INTO Users (Login, PasswordHash, Role)
                VALUES ('testuser', '{passwordHash}', 'User');
            ");

            // Получение ID только что созданного пользователя
            // ПРИМЕЧАНИЕ: В реальном приложении это делается более надежно, но для тестов подойдет.
            // Мы предполагаем, что это будет первый и единственный пользователь на момент миграции.
            var userId = 1;

            // Добавление тестовых счетов
            migrationBuilder.Sql(@$"
                INSERT INTO Accounts (UserId, Type, Balance, Name, Currency)
                VALUES ({userId}, 'Карта', 10000, 'Карта Сбербанк', 'RUB'),
                       ({userId}, 'Наличные', 5000, 'Кошелек', 'RUB');
            ");

            // Добавление тестовых категорий
            migrationBuilder.Sql(@"
                INSERT INTO Categories (Name, Type, Description)
                VALUES ('Зарплата', 'Income', ''), ('Продукты', 'Expense', ''), ('Транспорт', 'Expense', '');
            ");

            // Получение ID счетов и категорий
            var accountIdCard = 1;
            var accountIdCash = 2;
            var categoryIdSalary = 1;
            var categoryIdFood = 2;
            var categoryIdTransport = 3;

            // Добавление тестовых транзакций
            migrationBuilder.Sql(@$"
                INSERT INTO Transactions (AccountId, CategoryId, Type, Amount, Date, Comment)
                VALUES ({accountIdCard}, {categoryIdSalary}, 'Income', 50000, '{DateTime.Now.AddDays(-10):yyyy-MM-dd HH:mm:ss}', 'Аванс'),
                       ({accountIdCard}, {categoryIdFood}, 'Expense', 1500, '{DateTime.Now.AddDays(-5):yyyy-MM-dd HH:mm:ss}', 'Пятерочка'),
                       ({accountIdCash}, {categoryIdTransport}, 'Expense', 500, '{DateTime.Now.AddDays(-2):yyyy-MM-dd HH:mm:ss}', 'Такси');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Transactions");
        }
    }
}
