using System.Windows;
using Kopilka.BusinessLogic;
using Kopilka.DataAccess;

namespace Kopilka.FinanceManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly UserService _userService;
        private readonly KopilkaDbContext _dbContext;

        public MainWindow()
        {
            InitializeComponent();
            _dbContext = new KopilkaDbContext();
            _userService = new UserService(_dbContext);
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;
            var passwordConfirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordConfirm))
            {
                ShowError("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                var newUser = await _userService.RegisterUserAsync(login, password, passwordConfirm);
                MessageBox.Show($"Пользователь {newUser.Login} успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.ArgumentException ex)
            {
                ShowError(ex.Message);
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var login = LoginTextBox.Text;
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Пожалуйста, введите логин и пароль.");
                return;
            }

            var user = await _userService.LoginAsync(login, password);

            if (user != null)
            {
                var dashboard = new DashboardWindow(user);
                dashboard.Show();
                this.Close();
            }
            else
            {
                ShowError("Неверный логин или пароль.");
            }
        }

        private void ShowError(string message)
        {
            ErrorTextBlock.Text = message;
            ErrorBorder.Visibility = Visibility.Visible;
        }

        private void HideError()
        {
            ErrorBorder.Visibility = Visibility.Collapsed;
        }

        private void CloseErrorButton_Click(object sender, RoutedEventArgs e)
        {
            HideError();
        }

        private void Input_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            HideError();
        }

        private void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            HideError();
        }
    }
}
