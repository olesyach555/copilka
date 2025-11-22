using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class AddEditAccountWindow : Window
    {
        private readonly AccountService _accountService;
        private readonly int _userId;
        private Account _accountToEdit;

        // Конструктор для добавления нового счета
        public AddEditAccountWindow(AccountService accountService, int userId)
        {
            InitializeComponent();
            _accountService = accountService;
            _userId = userId;
            AccountBalanceTextBox.IsEnabled = true; // Разрешаем редактировать баланс при создании
        }

        // Конструктор для редактирования существующего счета
        public AddEditAccountWindow(AccountService accountService, int userId, Account accountToEdit)
        {
            InitializeComponent();
            _accountService = accountService;
            _userId = userId;
            _accountToEdit = accountToEdit;

            // Заполняем поля данными счета
            WindowTitle.Text = "Редактировать счет";
            AccountNameTextBox.Text = _accountToEdit.Name;
            AccountBalanceTextBox.Text = _accountToEdit.Balance.ToString();
            AccountBalanceTextBox.IsEnabled = false; // Запрещаем редактировать баланс напрямую
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(AccountNameTextBox.Text))
            {
                MessageBox.Show("Имя счета не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(AccountBalanceTextBox.Text, out decimal balance))
            {
                MessageBox.Show("Некорректный формат баланса.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_accountToEdit == null) // Режим добавления
            {
                var newAccount = new Account
                {
                    UserId = _userId,
                    Name = AccountNameTextBox.Text,
                    Balance = balance
                };
                await _accountService.AddAccountAsync(newAccount);
            }
            else // Режим редактирования
            {
                _accountToEdit.Name = AccountNameTextBox.Text;
                // Баланс не меняем напрямую, он должен меняться через транзакции
                await _accountService.UpdateAccountAsync(_accountToEdit);
            }

            DialogResult = true;
            Close();
        }
    }
}
