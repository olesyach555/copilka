using Kopilka.Shared;
using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class WelcomePage : Page
    {
        public WelcomePage()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var sampleTransactions = new List<Transaction>
            {
                // Sample Expenses
                new Transaction { Date = DateTime.Now.AddDays(-1), Amount = 250, Type = "Expense", Category = new Category { Name = "Продукты" }, Account = new Account { Name = "Основной счет" }, Comment = "Покупка в супермаркете" },
                new Transaction { Date = DateTime.Now.AddDays(-2), Amount = 1500, Type = "Expense", Category = new Category { Name = "Развлечения" }, Account = new Account { Name = "Основной счет" }, Comment = "Билеты в кино" },
                new Transaction { Date = DateTime.Now.AddDays(-3), Amount = 300, Type = "Expense", Category = new Category { Name = "Транспорт" }, Account = new Account { Name = "Основной счет" }, Comment = "Такси" },

                // Sample Incomes
                new Transaction { Date = DateTime.Now, Amount = 50000, Type = "Income", Category = new Category { Name = "Зарплата" }, Account = new Account { Name = "Зарплатный счет" }, Comment = "Аванс" },
                new Transaction { Date = DateTime.Now.AddDays(-5), Amount = 10000, Type = "Income", Category = new Category { Name = "Подарки" }, Account = new Account { Name = "Основной счет" }, Comment = "Подарок на день рождения" }
            };

            TransactionsListView.ItemsSource = sampleTransactions;
        }
    }
}
