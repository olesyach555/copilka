using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class AccountsViewModel : ObservableObject
    {
        public AccountService AccountService { get; }
        public int UserId { get; }

        public ObservableCollection<Account> Accounts { get; } = new();

        public AccountsViewModel(AccountService accountService, int userId)
        {
            AccountService = accountService;
            UserId = userId;
        }

        [RelayCommand]
        private async Task LoadAccountsAsync()
        {
            Accounts.Clear();
            var accounts = await AccountService.GetAccountsAsync(UserId);
            foreach (var acc in accounts)
            {
                Accounts.Add(acc);
            }
        }

        // События для открытия окон
        public event Action? RequestOpenAddAccountWindow;
        public event Action<Account>? RequestOpenEditAccountWindow;

        [RelayCommand]
        private void OpenAddAccount() => RequestOpenAddAccountWindow?.Invoke();

        [RelayCommand]
        private void OpenEditAccount(Account account) => RequestOpenEditAccountWindow?.Invoke(account);
    }
}
