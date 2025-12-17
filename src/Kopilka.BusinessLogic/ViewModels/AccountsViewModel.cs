using CommunityToolkit.Mvvm.Input;
using Kopilka.BusinessLogic.Services;
using Kopilka.BusinessLogic.ViewModels.Base;
using Kopilka.Shared;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic.ViewModels
{
    /// <summary>
    /// ViewModel для страницы "Счета".
    /// </summary>
    public partial class AccountsViewModel : ViewModelBase
    {
        private readonly AccountService _accountService; // Нужен для окна добавления
        private readonly StateService _stateService;
        public User CurrentUser { get; }

        public event Action RequestOpenAddAccountWindow;

        /// <summary>
        /// Коллекция счетов пользователя (получаем напрямую из StateService).
        /// </summary>
        public ObservableCollection<Account> Accounts => _stateService.Accounts;

        /// <summary>
        /// Общая сумма на всех счетах (проксируется из StateService).
        /// </summary>
        public decimal TotalBalance => _stateService.TotalBalance;

        public AccountsViewModel(AccountService accountService, User currentUser, StateService stateService)
        {
            _accountService = accountService;
            CurrentUser = currentUser;
            _stateService = stateService;

            // Подписываемся на изменения в StateService, чтобы UI всегда был актуальным
            _stateService.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(StateService.TotalBalance))
                {
                    OnPropertyChanged(nameof(TotalBalance));
                }
            };
        }

        /// <summary>
        /// Запрашивает у StateService полную перезагрузку данных.
        /// </summary>
        public async Task RefreshDataAsync()
        {
            await _stateService.ReloadAllDataAsync();
        }

        [RelayCommand]
        private void AddAccount()
        {
            RequestOpenAddAccountWindow?.Invoke();
        }
    }
}
