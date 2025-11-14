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

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Пожалуйста, введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existingUser = await _userService.GetUserByLoginAsync(login);
            if (existingUser != null)
            {
                MessageBox.Show("Пользователь с таким логином уже существует.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newUser = await _userService.RegisterUserAsync(login, password);
            MessageBox.Show($"Пользователь {newUser.Login} успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Логика входа будет реализована в следующей итерации.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
