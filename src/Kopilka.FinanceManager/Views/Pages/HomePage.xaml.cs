using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;

namespace Kopilka.FinanceManager.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly User _currentUser;

        public HomePage(User currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadHomePageDataAsync();
        }

        private async System.Threading.Tasks.Task LoadHomePageDataAsync()
        {
            try
            {
                using (var dbContext = new KopilkaDbContext())
                {
                    // Загрузка последних транзакций
                    var transactionService = new TransactionService(dbContext);
                    var transactions = await transactionService.GetRecentTransactionsAsync(_currentUser.Id, 15);
                    TransactionsListView.ItemsSource = transactions;

                    // Обновление общей суммы на счетах
                    await UpdateTotalBalanceAsync(dbContext);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке данных: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async System.Threading.Tasks.Task UpdateTotalBalanceAsync(KopilkaDbContext dbContext)
        {
            var totalBalance = (decimal)await dbContext.Accounts
                                              .Where(a => a.UserId == _currentUser.Id)
                                              .SumAsync(a => (double)a.Balance);
            TotalBalanceTextBlock.Text = $"{totalBalance:N2} ₽";
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAddTransactionWindow();
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAddTransactionWindow();
        }

        private void OpenAddTransactionWindow()
        {
            var addTransactionWindow = new AddTransactionWindow(_currentUser);
            if (addTransactionWindow.ShowDialog() == true)
            {
                // Если транзакция была успешно добавлена, обновляем все данные на странице
                _ = LoadHomePageDataAsync();
            }
        }
    }
}
