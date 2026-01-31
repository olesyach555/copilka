using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class ChangePasswordWindow : Window
    {
        private readonly AuthService? _authService;
        private readonly UserService? _userService;
        private readonly User? _currentUser;

        public ChangePasswordWindow(AuthService? authService, UserService? userService, User? currentUser)
        {
            InitializeComponent();
            _authService = authService;
            _userService = userService;
            _currentUser = currentUser;
        }

        private async void SavePasswordButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null || _authService == null || _userService == null) return;

            // 1. Проверяем старый пароль
            var user = await _authService.LoginAsync(_currentUser.Login, OldPasswordBox.Password);
            if (user == null)
            {
                MessageBox.Show("Старый пароль введен неверно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Проверяем, совпадают ли новые пароли
            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("Новые пароли не совпадают.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 3. Проверяем, что новый пароль не пустой
            if (string.IsNullOrWhiteSpace(NewPasswordBox.Password))
            {
                MessageBox.Show("Новый пароль не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 4. Обновляем пароль
            await _userService.ChangePasswordAsync(_currentUser.Id, NewPasswordBox.Password);
            MessageBox.Show("Пароль успешно изменен.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
            Close();
        }
    }
}
