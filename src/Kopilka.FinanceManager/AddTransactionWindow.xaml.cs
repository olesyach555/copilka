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
        private readonly TransactionService? _transactionService;
        private readonly KopilkaDbContext? _dbContext;
        private readonly User? _currentUser;
        private Category? _selectedCategory;
        private readonly Transaction? _editingTransaction;

        public AddTransactionWindow(User? user, string transactionType = "Expense")
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new KopilkaDbContext();
            _transactionService = new TransactionService(_dbContext);
            LoadInitialData();

            if (transactionType == "Income")
            {
                TransactionTypeTabControl.SelectedIndex = 1;
            }
        }

        public AddTransactionWindow(User? user, Transaction transactionToEdit)
        {
            InitializeComponent();
            _currentUser = user;
            _dbContext = new KopilkaDbContext();
            _transactionService = new TransactionService(_dbContext);
            _editingTransaction = transactionToEdit;

            LoadInitialData();
            LoadTransactionData();

            Title = "Редактирование операции";
            AddButton.Content = "Сохранить";
        }

        private void LoadInitialData()
        {
            if (_dbContext == null || _currentUser == null) return;
            AccountComboBox.ItemsSource = _dbContext.Accounts.Where(a => a.UserId == _currentUser.Id).ToList();
            LoadCategories("Expense");
        }

        private void LoadTransactionData()
        {
            if (_editingTransaction == null) return;

            AmountTextBox.Text = _editingTransaction.Amount.ToString("F2");
            DatePicker.SelectedDate = _editingTransaction.Date;
            CommentTextBox.Text = _editingTransaction.Comment;

            AccountComboBox.SelectedItem = ((System.Collections.Generic.List<Account>)AccountComboBox.ItemsSource)
                .FirstOrDefault(a => a.Id == _editingTransaction.AccountId);

            if (_editingTransaction.Type == "Income")
            {
                TransactionTypeTabControl.SelectedIndex = 1;
                LoadCategories("Income");
            }
            else
            {
                TransactionTypeTabControl.SelectedIndex = 0;
                LoadCategories("Expense");
            }

            if (_dbContext != null)
            {
                _selectedCategory = _dbContext.Categories.Find(_editingTransaction.CategoryId);
            }
        }

        private void LoadCategories(string type)
        {
            if (_dbContext == null) return;
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
                DatePicker.SelectedDate is DateTime date &&
                _currentUser != null &&
                _transactionService != null)
            {
                if (_editingTransaction == null) // Режим создания
                {
                    var newTransaction = new Transaction
                    {
                        Amount = amount,
                        Type = _selectedCategory.Type,
                        Date = date,
                        Comment = CommentTextBox.Text,
                        AccountId = account.Id,
                        CategoryId = _selectedCategory.Id,
                        UserId = _currentUser.Id
                    };
                    await _transactionService.AddTransactionAsync(newTransaction);
                }
                else // Режим редактирования
                {
                    _editingTransaction.Amount = amount;
                    _editingTransaction.Type = _selectedCategory.Type;
                    _editingTransaction.Date = date;
                    _editingTransaction.Comment = CommentTextBox.Text;
                    _editingTransaction.AccountId = account.Id;
                    _editingTransaction.CategoryId = _selectedCategory.Id;
                    await _transactionService.UpdateTransactionAsync(_editingTransaction);
                }
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля (сумма, категория, счет, дата) корректно.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
