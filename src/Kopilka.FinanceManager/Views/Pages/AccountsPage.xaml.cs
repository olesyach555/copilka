using System.Windows;
using System.Windows.Controls;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.DataAccess;

namespace Kopilka.FinanceManager.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для AccountsPage.xaml
    /// </summary>
    public partial class AccountsPage : Page
    {
        private readonly AccountsViewModel _viewModel;
        private readonly AccountService _accountService;

        public AccountsPage(AccountsViewModel viewModel, AccountService accountService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _accountService = accountService;
            DataContext = _viewModel;

            // Подписываемся на события из ViewModel
            _viewModel.RequestOpenAddAccountWindow += ViewModel_RequestOpenAddAccountWindow;
            _viewModel.RequestOpenEditAccountWindow += ViewModel_RequestOpenEditAccountWindow;
            // Отписываемся, когда страница выгружается
            Unloaded += (s, e) =>
            {
                _viewModel.RequestOpenAddAccountWindow -= ViewModel_RequestOpenAddAccountWindow;
                _viewModel.RequestOpenEditAccountWindow -= ViewModel_RequestOpenEditAccountWindow;
            };
        }

        private async void ViewModel_RequestOpenAddAccountWindow()
        {
            // Используем AccountService, переданный из MainWindow
            var addAccountWindow = new AddEditAccountWindow(_accountService, _viewModel.CurrentUser.Id);

            if (addAccountWindow.ShowDialog() == true)
            {
                // Если счет был успешно добавлен, просим ViewModel обновить данные.
                await _viewModel.RefreshDataAsync();
            }
        }

        private async void ViewModel_RequestOpenEditAccountWindow(Shared.Account account)
        {
            // Открываем то же окно, но передаем существующий счет для редактирования
            var editAccountWindow = new AddEditAccountWindow(_accountService, _viewModel.CurrentUser.Id, account);
            if (editAccountWindow.ShowDialog() == true)
            {
                await _viewModel.RefreshDataAsync();
            }
        }
    }
}
