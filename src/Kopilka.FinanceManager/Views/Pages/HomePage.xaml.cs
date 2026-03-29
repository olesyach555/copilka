using Kopilka.BusinessLogic;
using Kopilka.DataAccess;
using Kopilka.Shared;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private readonly User? _currentUser;
        private readonly TransactionService? _transactionService;

        public HomePage(User? currentUser, TransactionService? transactionService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _transactionService = transactionService;
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                TransactionsListView.ItemsSource = null;
                return;
            }

            try
            {
                var transactions = await _transactionService!.GetRecentTransactionsAsync(_currentUser.Id, 15);
                TransactionsListView.ItemsSource = transactions;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке транзакций: {ex.Message}",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Expense");
            addTransactionWindow.Owner = Window.GetWindow(this);
            if (addTransactionWindow.ShowDialog() == true)
            {
                // Обновляем UI после добавления
                HomePage_Loaded(this, new RoutedEventArgs());
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.LoadUserDataPublic();
            }
        }

        private void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null) return;

            var addTransactionWindow = new AddTransactionWindow(_currentUser, "Income");
            addTransactionWindow.Owner = Window.GetWindow(this);
            if (addTransactionWindow.ShowDialog() == true)
            {
                // Обновляем UI после добавления
                HomePage_Loaded(this, new RoutedEventArgs());
                var mainWindow = Window.GetWindow(this) as MainWindow;
                mainWindow?.LoadUserDataPublic();
            }
        }

        private async void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int transactionId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите удалить эту транзакцию?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        await _transactionService!.DeleteTransactionAsync(transactionId);

                        // Обновляем UI после удаления
                        HomePage_Loaded(this, new RoutedEventArgs());
                        var mainWindow = Window.GetWindow(this) as MainWindow;
                        mainWindow?.LoadUserDataPublic();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при удалении транзакции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Transaction transactionToEdit && _currentUser != null)
            {
                var editTransactionWindow = new AddTransactionWindow(_currentUser, transactionToEdit);
                editTransactionWindow.Owner = Window.GetWindow(this);
                if (editTransactionWindow.ShowDialog() == true)
                {
                    // Обновляем UI после редактирования
                    HomePage_Loaded(this, new RoutedEventArgs());
                    var mainWindow = Window.GetWindow(this) as MainWindow;
                    mainWindow?.LoadUserDataPublic();
                }
            }
        }
    }
}
