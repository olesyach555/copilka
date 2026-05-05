using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class TransactionsViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactions = new();

        [ObservableProperty]
        private decimal _totalBalance;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        public TransactionsViewModel(TransactionService transactionService, int userId)
        {
            _transactionService = transactionService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var list = await _transactionService.GetTransactionsAsync(_userId, StartDate, EndDate);
            Transactions = new ObservableCollection<Transaction>(list);
            TotalBalance = await _transactionService.GetTotalBalanceAsync(_userId);
        }

        [RelayCommand]
        private void ShowChart()
        {
            // Логика перехода к графику
        }
    }
}
