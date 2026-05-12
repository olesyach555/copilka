using System.Windows;
using System.Windows.Controls;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.Shared;

namespace Kopilka.FinanceManager
{
    public partial class LoginWindow : Window
    {
        private readonly AuthViewModel _viewModel;

        public LoginWindow(AuthViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
            _viewModel.OnAuthenticated += (user) =>
            {
                Dispatcher.Invoke(() =>
                {
                    this.DialogResult = true;
                    this.Tag = user;
                    this.Close();
                });
            };
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
            {
                _viewModel.Password = pb.Password;
            }
        }
    }
}
