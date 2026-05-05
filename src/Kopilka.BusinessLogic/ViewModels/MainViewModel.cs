using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly DebtService _debtService;
        private readonly GoalService _goalService;
        private readonly ReminderService _reminderService;
        private readonly int _userId;

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        public MainViewModel(TransactionService transactionService, DebtService debtService, GoalService goalService, ReminderService reminderService, int userId)
        {
            _transactionService = transactionService;
            _debtService = debtService;
            _goalService = goalService;
            _reminderService = reminderService;
            _userId = userId;

            NavigateToTransactions();
        }

        [RelayCommand]
        public void NavigateToTransactions()
        {
            var vm = new TransactionsViewModel(_transactionService, _userId);
            vm.RequestShowChart += (start, end) => NavigateToChart(start, end);
            _ = vm.LoadDataAsync();
            CurrentViewModel = vm;
        }

        private void NavigateToChart(DateTime start, DateTime end)
        {
            var vm = new ChartViewModel(_transactionService, _userId, start, end);
            _ = vm.LoadDataAsync();
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToDebts()
        {
            var vm = new DebtsViewModel(_debtService, _userId);
            _ = vm.LoadDebtsAsync();
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToGoals()
        {
            var vm = new GoalsViewModel(_goalService, _userId);
            _ = vm.LoadGoalsAsync();
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToReminders()
        {
            var vm = new RemindersViewModel(_reminderService, _userId);
            _ = vm.LoadRemindersAsync();
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToSettings()
        {
            CurrentViewModel = new FamilySettingsViewModel();
        }
    }
}
