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

        public LoginWindow(AuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = await _authService.LoginAsync(LoginTextBox.Text, PasswordBox.Password);
                if (user != null)
                {
                    var dbContext = new KopilkaDbContext();
                    var mainWindow = new MainWindow(user, dbContext);
                    mainWindow.Show();
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
                MessageBox.Show($"Пользователь {user.Login} успешно зарегистрирован!", "Регистрация успешна", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex) // Отлавливаем ошибки валидации
            {
                MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Пустые обработчики для совместимости с XAML
        private void Input_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void Password_PasswordChanged(object sender, RoutedEventArgs e) { }
        private void CloseErrorButton_Click(object sender, RoutedEventArgs e) { }
    }
}
