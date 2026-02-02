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
        private User? _currentUser;
        private readonly KopilkaDbContext? _dbContext;
        private readonly TransactionService? _transactionService;
        private readonly AccountService? _accountService;
        private readonly CategoryService? _categoryService;
        private readonly UserService? _userService;
        private readonly AuthService? _authService;

        public MainWindow(
            User? user,
            KopilkaDbContext? dbContext,
            TransactionService? transactionService,
            AccountService? accountService,
            CategoryService? categoryService,
            UserService? userService,
            AuthService? authService)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = dbContext;
            _transactionService = transactionService;
            _accountService = accountService;
            _categoryService = categoryService;
            _userService = userService;
            _authService = authService;

            LoadUserDataPublic();
            NavigateToHome();
        }

        public async void LoadUserDataPublic()
        {
            if (_currentUser != null && _transactionService != null)
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
            if (_transactionService != null)
                MainFrame.Navigate(new HomePage(_currentUser, _transactionService));
        }

        private void NavigateToAccounts()
        {
            if (_currentUser == null || _accountService == null) return;
            var accountsViewModel = new AccountsViewModel(_accountService, _currentUser.Id);
            MainFrame.Navigate(new AccountsPage(accountsViewModel));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) => NavigateToHome();
        private void AccountsButton_Click(object sender, RoutedEventArgs e) => NavigateToAccounts();
        private void ChartsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ChartsPage());
        private void CategoriesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null || _categoryService == null) return;
            MainFrame.Navigate(new CategoriesPage(_currentUser, _categoryService));
        }
        private void RegularPaymentsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegularPaymentsPage());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RemindersPage());
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null || _userService == null || _authService == null) return;
            MainFrame.Navigate(new SettingsPage(_currentUser, _userService, _authService));
        }

        private async void UserButton_Click(object sender, RoutedEventArgs e)
        {
            if (_authService == null) return;
            var loginWindow = new LoginWindow(_authService, _currentUser);
            var result = loginWindow.ShowDialog();

            if (result == true)
            {
                var lastUserId = SettingsService.GetLastUserId();
                User? user = null;
                if (lastUserId > 0)
                {
                    // Используем новый DbContext чтобы получить актуальные данные, если они изменились
                    using (var dbContext = new KopilkaDbContext())
                    {
                        user = await dbContext.Users.FindAsync(lastUserId);
                    }
                }
                _currentUser = user;

                LoadUserDataPublic();
                NavigateToHome();
            }
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Для добавления расхода необходимо войти в систему.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Expense");
            if (addTransactionWindow.ShowDialog() == true)
            {
                LoadUserDataPublic();
                NavigateToHome();
            }
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("Для добавления дохода необходимо войти в систему.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Income");
            if (addTransactionWindow.ShowDialog() == true)
            {
                LoadUserDataPublic();
                NavigateToHome();
            }
        }
    }
}
