using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly TransactionService _transactionService;
        private readonly CategoryService _categoryService;
        private readonly DebtService _debtService;
        private readonly GoalService _goalService;
        private readonly ReminderService _reminderService;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly int _userId;

        [ObservableProperty]
        private ViewModelBase? _currentViewModel;

        public event Action? RequestLogout;
        public event Action<string>? ShowNotification;

        public MainViewModel(TransactionService transactionService,
                             CategoryService categoryService,
                             DebtService debtService,
                             GoalService goalService,
                             ReminderService reminderService,
                             AuthService authService,
                             UserService userService,
                             int userId)
        {
            _transactionService = transactionService;
            _categoryService = categoryService;
            _debtService = debtService;
            _goalService = goalService;
            _reminderService = reminderService;
            _authService = authService;
            _userService = userService;
            _userId = userId;

            NavigateToTransactions();
            _ = CheckRemindersAsync();
        }

        private async Task CheckRemindersAsync()
        {
            var reminders = await _reminderService.GetActiveRemindersAsync(_userId);
            var today = DateTime.Today;
            var upcoming = reminders.Where(r => r.ReminderDate.Date == today).ToList();
            if (upcoming.Any())
            {
                ShowNotification?.Invoke($"На сегодня запланировано {upcoming.Count} дел(а)!");
            }
        }

        [RelayCommand]
        public void NavigateToTransactions()
        {
            var vm = new TransactionsViewModel(_transactionService, _categoryService, _userId);
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
            var vm = new RemindersViewModel(_reminderService, _categoryService, _userId);
            _ = vm.LoadRemindersAsync();
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void NavigateToSettings()
        {
            var vm = new FamilySettingsViewModel(_authService, _userService, _userId);
            CurrentViewModel = vm;
        }

        [RelayCommand]
        public void Logout()
        {
            RequestLogout?.Invoke();
        }
    }
}
