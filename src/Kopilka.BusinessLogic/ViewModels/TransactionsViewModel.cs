using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class TransactionsViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<Transaction> _transactions = new();

        [ObservableProperty]
        private ObservableCollection<Category> _categories = new();

        [ObservableProperty]
        private Category? _selectedCategory;

        [ObservableProperty]
        private decimal _totalBalance;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Now.AddMonths(-1);

        [ObservableProperty]
        private DateTime _endDate = DateTime.Now;

        public event Action<DateTime, DateTime>? RequestShowChart;
        public Func<Transaction, List<Category>, bool?>? ShowEditDialog;
        public Func<string, string, bool>? ShowConfirmDialog;

        public TransactionsViewModel(TransactionService transactionService, CategoryService categoryService, int userId)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadDataAsync()
        {
            var list = await _transactionService.GetTransactionsAsync(_userId, StartDate, EndDate, SelectedCategory?.Id);
            Transactions = new ObservableCollection<Transaction>(list);
            TotalBalance = await _transactionService.GetTotalBalanceAsync(_userId);

            var cats = await _categoryService.GetCategoriesAsync(_userId);
            Categories = new ObservableCollection<Category>(cats);
        }

        [RelayCommand]
        public async Task AddTransactionAsync()
        {
            var transaction = new Transaction { UserId = _userId, Date = DateTime.Now };
            if (ShowEditDialog?.Invoke(transaction, Categories.ToList()) == true)
            {
                await _transactionService.AddTransactionAsync(transaction);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task EditTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;
            if (ShowEditDialog?.Invoke(transaction, Categories.ToList()) == true)
            {
                await _transactionService.UpdateTransactionAsync(transaction);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task DeleteTransactionAsync(Transaction transaction)
        {
            if (transaction == null) return;
            if (ShowConfirmDialog?.Invoke($"Удалить транзакцию на сумму {transaction.Amount}?", "Подтверждение") == true)
            {
                await _transactionService.DeleteTransactionAsync(transaction.Id);
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        private void ShowChart()
        {
            RequestShowChart?.Invoke(StartDate, EndDate);
        }
    }
}
