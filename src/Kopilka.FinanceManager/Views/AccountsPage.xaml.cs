using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views
{
    public partial class AccountsPage : UserControl
    {
        private readonly AccountService _accountService;
        private readonly User _currentUser;

        public AccountsPage(User currentUser, AccountService accountService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _accountService = accountService;
            Loaded += AccountsPage_Loaded;
        }

        private void AccountsPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAccounts();
        }

        private async void LoadAccounts()
        {
            var accounts = await _accountService.GetAccountsAsync(_currentUser.Id);
            AccountsListView.ItemsSource = accounts;
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var addAccountWindow = new AddEditAccountWindow(_accountService, _currentUser.Id);
            addAccountWindow.Owner = Window.GetWindow(this);
            if (addAccountWindow.ShowDialog() == true)
            {
                LoadAccounts();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Account account)
            {
                var editAccountWindow = new AddEditAccountWindow(_accountService, _currentUser.Id, account);
                editAccountWindow.Owner = Window.GetWindow(this);
                if (editAccountWindow.ShowDialog() == true)
                {
                    LoadAccounts();
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Account account)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить счет '{account.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await _accountService.DeleteAccountAsync(account.Id);
                    LoadAccounts();
                }
            }
        }
    }
}
