using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class LoginWindow : Window
    {
        private readonly AuthService _authService;
        private readonly User _currentUser;

        public LoginWindow(AuthService authService, User currentUser = null)
        {
            InitializeComponent();
            _authService = authService;
            _currentUser = currentUser;

            if (_currentUser != null)
            {
                LoginGrid.Visibility = Visibility.Collapsed;
                LogoutButton.Visibility = Visibility.Visible;
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = await _authService.LoginAsync(LoginTextBox.Text, PasswordBox.Password);
                if (user != null)
                {
                    Kopilka.FinanceManager.Properties.Settings.Default.LastUserId = user.Id;
                    Kopilka.FinanceManager.Properties.Settings.Default.Save();
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Пароли не совпадают.", "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var user = await _authService.RegisterUserAsync(LoginTextBox.Text, PasswordBox.Password);

                Kopilka.FinanceManager.Properties.Settings.Default.LastUserId = user.Id;
                Kopilka.FinanceManager.Properties.Settings.Default.Save();

                DialogResult = true;
                Close();
            }
            catch (Exception ex) // Отлавливаем ошибки валидации
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            Kopilka.FinanceManager.Properties.Settings.Default.LastUserId = 0;
            Kopilka.FinanceManager.Properties.Settings.Default.Save();
            DialogResult = true;
            Close();
        }

        // Пустые обработчики для совместимости с XAML
        private void Input_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e) { }
        private void CloseErrorButton_Click(object sender, RoutedEventArgs e) { }
    }
}
