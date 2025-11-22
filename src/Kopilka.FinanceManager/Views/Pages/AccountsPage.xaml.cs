using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccountsPage.xaml
    /// </summary>
    public partial class AccountsPage : Page
    {
        private readonly KopilkaDbContext _context;
        private readonly User _currentUser;
        private readonly AccountService _accountService;

        public AccountsPage(KopilkaDbContext context, User currentUser)
        {
            InitializeComponent();
            _context = context;
            _currentUser = currentUser;
            _accountService = new AccountService(_context); // Инициализируем сервис

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            var accounts = _context.Accounts
                .Where(a => a.UserId == _currentUser.Id)
                .ToList();
            AccountsListView.ItemsSource = accounts;
        }

        private void EditAccount_Click(object sender, RoutedEventArgs e)
        {
            // Получаем счет из контекста данных кнопки
            if ((sender as FrameworkElement)?.DataContext is Account accountToEdit)
            {
                var editWindow = new AddEditAccountWindow(_accountService, _currentUser.Id, accountToEdit)
                {
                    Owner = Window.GetWindow(this)
                };

                if (editWindow.ShowDialog() == true)
                {
                    // Если окно было закрыто с успехом (нажата кнопка "Сохранить"),
                    // обновляем список счетов
                    LoadAccounts();
                }
            }
        }

        private void AddAccount_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddEditAccountWindow(_accountService, _currentUser.Id)
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() == true)
            {
                // Обновляем список после добавления
                LoadAccounts();
            }
        }
    }
}
