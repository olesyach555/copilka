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

        public AccountsPage(AccountsViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            // Подписываемся на событие из ViewModel
            _viewModel.RequestOpenAddAccountWindow += ViewModel_RequestOpenAddAccountWindow;
            // Отписываемся, когда страница выгружается
            Unloaded += (s, e) => _viewModel.RequestOpenAddAccountWindow -= ViewModel_RequestOpenAddAccountWindow;
        }

        private async void ViewModel_RequestOpenAddAccountWindow()
        {
            // Теперь View отвечает за создание и отображение View-элементов.
            // Мы получаем AccountService "снаружи", так как у ViewModel его нет в публичных свойствах.
            // В более крупной системе это решалось бы через Dependency Injection.
            var accountService = new AccountService(new KopilkaDbContext());
            var addAccountWindow = new AddEditAccountWindow(accountService, _viewModel.CurrentUser.Id);

            if (addAccountWindow.ShowDialog() == true)
            {
                // Если счет был успешно добавлен, просим ViewModel обновить данные.
                await _viewModel.RefreshDataAsync();
            }
        }
    }
}
