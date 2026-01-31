using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class AddEditAccountWindow : Window
    {
        private readonly AccountService? _accountService;
        private readonly int _userId;
        private Account? _accountToEdit; // Поле теперь nullable

        // Конструктор для добавления нового счета
        public AddEditAccountWindow(AccountService? accountService, int userId)
        {
            InitializeComponent();
            _accountService = accountService;
            _userId = userId;
            AccountBalanceTextBox.IsEnabled = true; // Разрешаем редактировать баланс при создании
        }

        // Конструктор для редактирования существующего счета
        public AddEditAccountWindow(AccountService? accountService, int userId, Account accountToEdit)
        {
            InitializeComponent();
            _accountService = accountService;
            _userId = userId;
            _accountToEdit = accountToEdit;

            // Заполняем поля данными счета
            WindowTitle.Text = "Редактировать счет";
            AccountNameTextBox.Text = _accountToEdit.Name;
            AccountBalanceTextBox.Text = _accountToEdit.Balance.ToString();
            // Включаем редактирование баланса согласно новым требованиям
            AccountBalanceTextBox.IsEnabled = true;
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text))
            {
                MessageBox.Show("Имя счета не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Используем CultureInfo.InvariantCulture для корректного парсинга
            if (!decimal.TryParse(AccountBalanceTextBox.Text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal balance))
            {
                MessageBox.Show("Некорректный формат баланса. Используйте точку в качестве десятичного разделителя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_accountToEdit == null) // Режим добавления
            {
                var newAccount = new Account
                {
                    UserId = _userId,
                    Name = AccountNameTextBox.Text,
                    Balance = balance,
                    Type = "Карта", // Устанавливаем значение по умолчанию
                    Currency = "RUB"   // Устанавливаем значение по умолчанию
                };
                await _accountService!.AddAccountAsync(newAccount);
            }
            else // Режим редактирования
            {
                _accountToEdit.Name = AccountNameTextBox.Text;
                // Разрешаем прямое изменение баланса
                _accountToEdit.Balance = balance;
                await _accountService!.UpdateAccountAsync(_accountToEdit);
            }

            DialogResult = true;
            Close();
        }
    }
}
