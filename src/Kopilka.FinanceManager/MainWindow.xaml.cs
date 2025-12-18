using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.Services;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.DataAccess;
using Kopilka.FinanceManager.Views;
using Kopilka.FinanceManager.Views.Pages;
using Kopilka.Shared;
using System.Linq;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly User _currentUser;
        private readonly KopilkaDbContext _dbContext;
        private readonly TransactionService _transactionService;
        private readonly AccountService _accountService;
        private readonly CategoryService _categoryService;
        private readonly UserService _userService;
        private readonly AuthService _authService;

        private readonly StateService _stateService;

        public MainWindow(User user, KopilkaDbContext dbContext)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = dbContext;

            // Инициализация сервисов
            _transactionService = new TransactionService(_dbContext);
            _accountService = new AccountService(_dbContext);
            _categoryService = new CategoryService(_dbContext);
            _userService = new UserService(_dbContext);
            _authService = new AuthService(_dbContext);

            // Создание и инициализация единого StateService
            _stateService = new StateService(_accountService, _transactionService);

            // Загружаем данные асинхронно
            Loaded += async (s, e) =>
            {
                try
                {
                    await _stateService.InitializeAsync(_currentUser);
                    LoadUserData();
                    NavigateToHome();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show($"Произошла критическая ошибка при загрузке данных: {ex.Message}. Приложение может работать некорректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    // Можно также добавить логирование ошибки
                }
            };
        }

        private void LoadUserData()
        {
            UsernameTextBlock.Text = _currentUser.Login;
            if (!string.IsNullOrEmpty(_currentUser.Login))
            {
                UserInitialTextBlock.Text = _currentUser.Login[0].ToString().ToUpper();
            }
            // Баланс теперь берем из StateService
            BalanceTextBlock.Text = $"Остаток: {_stateService.TotalBalance:C}";
            _stateService.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(StateService.TotalBalance))
                {
                    BalanceTextBlock.Text = $"Остаток: {_stateService.TotalBalance:C}";
                }
            };
        }

        private void NavigateToHome()
        {
            var homeViewModel = new HomePageViewModel(_stateService, _currentUser);
            MainFrame.Navigate(new HomePage(homeViewModel)); // Передаем ViewModel в HomePage
        }

        private void NavigateToAccounts()
        {
            var accountsViewModel = new AccountsViewModel(_accountService, _currentUser, _stateService);
            MainFrame.Navigate(new AccountsPage(accountsViewModel, _accountService));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) => NavigateToHome();
        private void AccountsButton_Click(object sender, RoutedEventArgs e) => NavigateToAccounts();
        private void ChartsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new ChartsPage());
        private void CategoriesButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new CategoriesPage(_currentUser, _categoryService));
        private void RegularPaymentsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RegularPaymentsPage());
        private void RemindersButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new RemindersPage());
        private void SettingsButton_Click(object sender, RoutedEventArgs e) => MainFrame.Navigate(new SettingsPage(_currentUser, _userService, _authService));
    }
}
