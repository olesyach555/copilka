using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly User? _currentUser;

        public HomePage(User? currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                TransactionsListView.ItemsSource = null;
                return;
            }

            try
            {
                // DbContext создается для каждой операции и автоматически освобождается
                using (var dbContext = new KopilkaDbContext())
                {
                    var transactionService = new TransactionService(dbContext);
                    var transactions = await transactionService.GetRecentTransactionsAsync(_currentUser.Id, 15);
                    TransactionsListView.ItemsSource = transactions;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке транзакций: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Expense");
            addTransactionWindow.Owner = Window.GetWindow(this);
            if (addTransactionWindow.ShowDialog() == true)
            {
                // Обновляем список транзакций после добавления новой
                HomePage_Loaded(this, new RoutedEventArgs());
            }
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Income");
            addTransactionWindow.Owner = Window.GetWindow(this);
            if (addTransactionWindow.ShowDialog() == true)
            {
                // Обновляем список транзакций после добавления новой
                HomePage_Loaded(this, new RoutedEventArgs());
            }
        }
    }
}
