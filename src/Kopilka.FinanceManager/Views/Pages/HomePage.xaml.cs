using Kopilka.Shared;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class HomePage : Page
    {
        public HomePage(List<Transaction> income, List<Transaction> expenses)
        {
            InitializeComponent();
            var allTransactions = new List<Transaction>();
            allTransactions.AddRange(income);
            allTransactions.AddRange(expenses);
            TransactionsListView.ItemsSource = allTransactions;
        }
    }
}
