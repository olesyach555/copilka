using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Linq;
using System.Windows;

namespace Kopilka.FinanceManager
{
    public partial class AddTransactionWindow : Window
    {
        private readonly TransactionService _transactionService;
        private readonly KopilkaDbContext _dbContext;
        private readonly User _currentUser;

        public AddTransactionWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new KopilkaDbContext();
            _transactionService = new TransactionService(_dbContext);
            LoadComboBoxes();
        }

        private void LoadComboBoxes()
        {
            AccountComboBox.ItemsSource = _dbContext.Accounts.Where(a => a.UserId == _currentUser.Id).ToList();
            CategoryComboBox.ItemsSource = _dbContext.Categories.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out var amount) &&
                CategoryComboBox.SelectedItem is Category category &&
                AccountComboBox.SelectedItem is Account account &&
                DatePicker.SelectedDate is DateTime date)
            {
                var transaction = new Transaction
                {
                    Amount = amount,
                    CategoryId = category.Id,
                    Category = category,
                    AccountId = account.Id,
                    Date = date
                };

                await _transactionService.AddTransactionAsync(transaction);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
