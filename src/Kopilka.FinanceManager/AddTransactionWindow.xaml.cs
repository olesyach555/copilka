using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager
{
    public partial class AddTransactionWindow : Window
    {
        private readonly TransactionService _transactionService;
        private readonly KopilkaDbContext _dbContext;
        private readonly User _currentUser;
        private Category? _selectedCategory; // Поле теперь nullable

        public AddTransactionWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new KopilkaDbContext();
            _transactionService = new TransactionService(_dbContext);
            LoadInitialData();
        }

        private void LoadInitialData()
        {
            AccountComboBox.ItemsSource = _dbContext.Accounts.Where(a => a.UserId == _currentUser.Id).ToList();
            if (AccountComboBox.Items.Count > 0)
            {
                AccountComboBox.SelectedIndex = 0;
            }
            LoadCategories("Expense");
        }

        private void LoadCategories(string type)
        {
            CategoryItemsControl.ItemsSource = _dbContext.Categories.Where(c => c.Type == type).ToList();
        }

        private void TransactionTypeTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source is TabControl)
            {
                var tab = (TabItem)TransactionTypeTabControl.SelectedItem;
                if (tab.Header.ToString() == "РАСХОДЫ")
                {
                    LoadCategories("Expense");
                }
                else
                {
                    LoadCategories("Income");
                }
            }
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Category category)
            {
                _selectedCategory = category;
                // Можно добавить визуальную обратную связь, например, изменение цвета кнопки
            }
        }

        private void TodayButton_Click(object sender, RoutedEventArgs e)
        {
            DatePicker.SelectedDate = DateTime.Now;
        }

        private void YesterdayButton_Click(object sender, RoutedEventArgs e)
        {
            DatePicker.SelectedDate = DateTime.Now.AddDays(-1);
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountTextBox.Text, out var amount) &&
                _selectedCategory != null &&
                AccountComboBox.SelectedItem is Account account &&
                DatePicker.SelectedDate is DateTime date)
            {
                var transaction = new Transaction
                {
                    Amount = amount,
                    Type = _selectedCategory.Type,
                    Date = date,
                    Comment = CommentTextBox.Text,
                    AccountId = account.Id,
                    CategoryId = _selectedCategory.Id,
                    UserId = _currentUser.Id
                };

                await _transactionService.AddTransactionAsync(transaction);
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля (сумма, категория, счет, дата) корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
