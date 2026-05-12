using System.Windows;
using Kopilka.BusinessLogic;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;
using Kopilka.FinanceManager.Views.Windows;

namespace Kopilka.FinanceManager
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;

            _viewModel.RequestLogout += () =>
            {
                var app = (App)Application.Current;
                app.ShowLoginWindow();
                this.Close();
            };

            _viewModel.ShowNotification += (msg) =>
            {
                MessageBox.Show(msg, "Напоминание", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            SetupViewModelDialogs();
        }

        private void SetupViewModelDialogs()
        {
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.CurrentViewModel))
                {
                    AttachDialogs(_viewModel.CurrentViewModel);
                }
            };
            AttachDialogs(_viewModel.CurrentViewModel);
        }

        private void AttachDialogs(ViewModelBase? vm)
        {
            if (vm is TransactionsViewModel tvm)
            {
                tvm.ShowEditDialog = (t, cats) => new TransactionEditWindow(t, cats) { Owner = this }.ShowDialog();
                tvm.ShowConfirmDialog = (msg, title) => MessageBox.Show(msg, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
            else if (vm is DebtsViewModel dvm)
            {
                dvm.ShowEditDialog = (d) => new DebtEditWindow(d) { Owner = this }.ShowDialog();
                dvm.ShowConfirmDialog = (msg, title) => MessageBox.Show(msg, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
            else if (vm is GoalsViewModel gvm)
            {
                gvm.ShowEditDialog = (g) => new GoalEditWindow(g) { Owner = this }.ShowDialog();
                gvm.ShowConfirmDialog = (msg, title) => MessageBox.Show(msg, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
                gvm.ShowPaymentDialog = () =>
                {
                    var win = new PaymentAddWindow { Owner = this };
                    if (win.ShowDialog() == true) return win.Amount;
                    return null;
                };
            }
            else if (vm is RemindersViewModel rvm)
            {
                rvm.ShowEditDialog = (r, cats) => new ReminderEditWindow(r, cats) { Owner = this }.ShowDialog();
                rvm.ShowConfirmDialog = (msg, title) => MessageBox.Show(msg, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
            }
            else if (vm is FamilySettingsViewModel fvm)
            {
                fvm.ShowFamilyActionDialog = () =>
                {
                    var win = new FamilyActionWindow { Owner = this };
                    if (win.ShowDialog() == true) return new Tuple<string, string, bool>(win.FamilyName, win.Password, win.IsCreate);
                    return null;
                };
                fvm.ShowErrorDialog = (msg, title) => MessageBox.Show(msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Logout();
        }
    }
}
