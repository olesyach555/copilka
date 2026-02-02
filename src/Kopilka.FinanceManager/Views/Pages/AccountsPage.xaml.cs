using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class AccountsPage : Page
    {
        private readonly AccountsViewModel? _viewModel;

        public AccountsPage(AccountsViewModel? viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            if (_viewModel != null)
            {
                _viewModel.RequestOpenAddAccountWindow += OnRequestOpenAddAccountWindow;
                _viewModel.RequestOpenEditAccountWindow += OnRequestOpenEditAccountWindow;
                Loaded += async (s, e) => await _viewModel.LoadAccountsCommand.ExecuteAsync(null);
            }
        }

        private void OnRequestOpenAddAccountWindow()
        {
            if (_viewModel == null) return;
            var addWindow = new AddEditAccountWindow(_viewModel.AccountService, _viewModel.UserId)
            {
                Owner = Window.GetWindow(this)
            };

            if (addWindow.ShowDialog() == true)
            {
                _viewModel.LoadAccountsCommand.Execute(null);
            }
        }

        private void OnRequestOpenEditAccountWindow(Account accountToEdit)
        {
            if (_viewModel == null) return;
            var editWindow = new AddEditAccountWindow(_viewModel.AccountService, _viewModel.UserId, accountToEdit)
            {
                Owner = Window.GetWindow(this)
            };

            if (editWindow.ShowDialog() == true)
            {
                _viewModel.LoadAccountsCommand.Execute(null);
            }
        }
    }
}
