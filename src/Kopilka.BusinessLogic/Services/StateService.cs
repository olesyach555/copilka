using CommunityToolkit.Mvvm.ComponentModel;
using Kopilka.Shared;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Kopilka.BusinessLogic.Services
{
    /// <summary>
    /// Централизованный сервис для управления состоянием приложения в памяти.
    /// Обеспечивает синхронизацию данных между разными ViewModel.
    /// </summary>
    public partial class StateService : ObservableObject
    {
        private readonly AccountService _accountService;
        private readonly TransactionService _transactionService;
        private User _currentUser;

        /// <summary>
        /// Коллекция счетов, доступная для всех частей приложения.
        /// </summary>
        public ObservableCollection<Account> Accounts { get; } = new ObservableCollection<Account>();

        /// <summary>
        /// Коллекция последних транзакций для отображения на главной странице.
        /// </summary>
        public ObservableCollection<Transaction> Transactions { get; } = new ObservableCollection<Transaction>();

        [ObservableProperty]
        private decimal _totalBalance;

        public StateService(AccountService accountService, TransactionService transactionService)
        {
            _accountService = accountService;
            _transactionService = transactionService;
        }

        /// <summary>
        /// Инициализирует сервис данными текущего пользователя.
        /// Должен вызываться один раз после аутентификации.
        /// </summary>
        public async Task InitializeAsync(User user)
        {
            _currentUser = user;
            await ReloadAllDataAsync();
        }

        /// <summary>
        /// Полностью перезагружает все данные (счета и баланс) из базы данных.
        /// </summary>
        public async Task ReloadAllDataAsync()
        {
            if (_currentUser == null) return;

            // Загружаем счета
            var userAccounts = await _accountService.GetAccountsAsync(_currentUser.Id);
            Accounts.Clear();
            foreach (var account in userAccounts)
            {
                Accounts.Add(account);
            }

            // Загружаем общий баланс
            TotalBalance = await _transactionService.GetTotalBalanceAsync(_currentUser.Id);

            // Загружаем транзакции
            var userTransactions = await _transactionService.GetRecentTransactionsAsync(_currentUser.Id, 50); // Загружаем 50 последних транзакций
            Transactions.Clear();
            foreach (var transaction in userTransactions) // Коллекция уже отсортирована и ограничена по количеству
            {
                Transactions.Add(transaction);
            }
        }
    }
}
