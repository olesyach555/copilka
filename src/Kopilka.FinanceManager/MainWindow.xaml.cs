using System.Windows;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;
        private readonly AuthService _authService;
        private readonly TransactionService _transactionService;
        private readonly DebtService _debtService;
        private readonly GoalService _goalService;
        private readonly ReminderService _reminderService;

        private User? _currentUser;

        public MainWindow(MainViewModel viewModel,
                          AuthService authService,
                          TransactionService transactionService,
                          DebtService debtService,
                          GoalService goalService,
                          ReminderService reminderService)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _authService = authService;
            _transactionService = transactionService;
            _debtService = debtService;
            _goalService = goalService;
            _reminderService = reminderService;

            DataContext = _viewModel;

            // В реальности тут должно быть открытие окна входа
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _currentUser = null;
            // Перейти на страницу входа
        }
    }
}
