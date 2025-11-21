using Kopilka.Shared;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views
{
    public partial class HomePage : UserControl
    {
        public HomePage()
        {
            InitializeComponent();
        }

        // Конструктор, принимающий данные о транзакциях
        public HomePage(List<Transaction> income, List<Transaction> expenses)
        {
            InitializeComponent();
            IncomeListView.ItemsSource = income;
            ExpenseListView.ItemsSource = expenses;
        }
    }
}
