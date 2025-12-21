using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.DataAccess;
using Kopilka.FinanceManager.Views;
using Kopilka.FinanceManager.Views.Pages;
using Kopilka.Shared;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private User _currentUser;
        private readonly KopilkaDbContext _dbContext;
        private readonly TransactionService _transactionService;
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public MainWindow(User user, KopilkaDbContext dbContext)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = dbContext;

            _transactionService = new TransactionService(_dbContext);
            _accountService = new AccountService(_dbContext);
            _categoryService = new CategoryService(_dbContext);
            _userService = new UserService(_dbContext);
            _authService = new AuthService(_dbContext);

            LoadUserData();
            NavigateToHome();
        }

        private async void LoadUserData()
        {
            if (_currentUser != null)
            {
                UsernameTextBlock.Text = _currentUser.Login;
                if (!string.IsNullOrEmpty(_currentUser.Login))
                {
                    UserInitialTextBlock.Text = _currentUser.Login[0].ToString().ToUpper();
                }

                var balance = await _transactionService.GetTotalBalanceAsync(_currentUser.Id);
                BalanceTextBlock.Text = $"Остаток: {balance:C}";
            }
            else
            {
                UsernameTextBlock.Text = "Гость";
                UserInitialTextBlock.Text = "Г";
                BalanceTextBlock.Text = "Остаток: 0,00 ₽";
            }
        }

        private void NavigateToHome()
        {
            MainFrame.Navigate(new HomePage(_currentUser));
        }

        private void NavigateToAccounts()
        {
            if (_currentUser == null) return;
            var accountsViewModel = new AccountsViewModel(_accountService, _currentUser.Id);
            MainFrame.Navigate(new AccountsPage(accountsViewModel));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) => NavigateToHome();
        private void AccountsButton_Click(object sender, RoutedEventArgs e) => NavigateToAccounts();
        private void ChartsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ChartsPage());
        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;
            MainFrame.Navigate(new CategoriesPage(_currentUser, _categoryService));
        }
        private void RegularPaymentsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegularPaymentsPage());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RemindersPage());
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;
            MainFrame.Navigate(new SettingsPage(_currentUser, _userService, _authService));
        }

        private async void UserButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(_authService, _currentUser);
            var result = loginWindow.ShowDialog();

            if (result == true)
            {
                var lastUserId = Kopilka.FinanceManager.Properties.Settings.Default.LastUserId;
                User user = null;
                if (lastUserId > 0)
                {
                    // Используем новый DbContext чтобы получить актуальные данные, если они изменились
                    using (var dbContext = new KopilkaDbContext())
                    {
                        user = await dbContext.Users.FindAsync(lastUserId);
                    }
                }
                _currentUser = user;

                LoadUserData();
                NavigateToHome();
            }
        }
    }
}
