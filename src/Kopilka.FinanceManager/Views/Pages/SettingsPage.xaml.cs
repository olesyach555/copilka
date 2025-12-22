using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class SettingsPage : Page // Изменено с UserControl на Page
    {
        private readonly UserService? _userService;
        private readonly AuthService? _authService;
        private readonly User? _currentUser;

        public SettingsPage(User currentUser, UserService userService, AuthService authService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _userService = userService;
            _authService = authService;
            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null)
            {
                LoginTextBlock.Text = _currentUser.Login;
                EmailTextBox.Text = _currentUser.Email;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser != null && _userService != null)
            {
                _currentUser.Email = EmailTextBox.Text;
                await _userService.UpdateUserAsync(_currentUser);
                MessageBox.Show("Изменения успешно сохранены.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangePasswordButton_Click(object sender, RoutedEventArgs e)
        {
             if (_authService != null && _userService != null && _currentUser != null)
            {
                var changePasswordWindow = new ChangePasswordWindow(_authService, _userService, _currentUser);
                changePasswordWindow.Owner = Window.GetWindow(this);
                changePasswordWindow.ShowDialog();
            }
        }
    }
}
