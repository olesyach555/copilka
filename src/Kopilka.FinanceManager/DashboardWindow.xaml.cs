using Kopilka.Shared;
using System.Windows;
using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using System.Threading.Tasks;
using System;
using System.Windows.Controls;

namespace Kopilka.FinanceManager
{
    public partial class DashboardWindow : Window
    {
        private readonly User _currentUser;
        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;
        private readonly KopilkaDbContext _dbContext;

        public DashboardWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new KopilkaDbContext();
            _accountService = new AccountService(_dbContext);
            _transactionService = new TransactionService(_dbContext);
            Loaded += async (sender, e) => await LoadInitialDataAsync();
        }

        private async Task LoadInitialDataAsync()
        {
            // Загружаем общий баланс и счета
            var balance = await _accountService.GetUserBalanceAsync(_currentUser.Id);
            TotalBalanceTextBlock.Text = $"{balance:N2} ₽";
            var accounts = await _accountService.GetUserAccountsAsync(_currentUser.Id);
            AccountsListView.ItemsSource = accounts;

            // Устанавливаем период по умолчанию (последняя неделя) и загружаем транзакции
            await LoadTransactionDataAsync(DateTime.Now.AddDays(-7), DateTime.Now);
        }

        private async Task LoadTransactionDataAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null || endDate == null) return;

            var income = await _transactionService.GetTotalIncomeAsync(_currentUser.Id, startDate.Value, endDate.Value);
            var expenses = await _transactionService.GetTotalExpensesAsync(_currentUser.Id, startDate, endDate);

            IncomeTextBlock.Text = $"{income:N2} ₽";
            ExpensesTextBlock.Text = $"{expenses:N2} ₽";
        }

        private async void DateFilterButton_Click(object sender, RoutedEventArgs e)
        {
            var dateFilterWindow = new DateFilterWindow();
            dateFilterWindow.Owner = this;
            if (dateFilterWindow.ShowDialog() == true)
            {
                await LoadTransactionDataAsync(dateFilterWindow.StartDate, dateFilterWindow.EndDate);
            }
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // В будущем здесь можно будет, например, сбрасывать фильтры
        }

        private void DashboardWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void AddTransactionButton_Click(object sender, RoutedEventArgs e)
        {
            var addTransactionWindow = new AddTransactionWindow(_currentUser);
            addTransactionWindow.Owner = this;
            if (addTransactionWindow.ShowDialog() == true)
            {
                await LoadInitialDataAsync();
            }
        }
    }
}
