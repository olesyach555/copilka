using System.Windows;
using Kopilka.Shared;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class TransactionEditWindow : Window
    {
        public Transaction Transaction { get; private set; }

        public TransactionEditWindow(Transaction transaction, List<Category> categories)
        {
            InitializeComponent();
            Transaction = transaction;

            AmountBox.Text = transaction.Amount.ToString();
            DatePicker.SelectedDate = transaction.Date == default ? DateTime.Now : transaction.Date;
            CommentBox.Text = transaction.Comment;

            CategoryCombo.ItemsSource = categories;
            if (transaction.CategoryId != 0)
            {
                CategoryCombo.SelectedItem = categories.FirstOrDefault(c => c.Id == transaction.CategoryId);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountBox.Text, out decimal amount) && CategoryCombo.SelectedItem is Category category)
            {
                Transaction.Amount = amount;
                Transaction.Date = DatePicker.SelectedDate ?? DateTime.Now;
                Transaction.CategoryId = category.Id;
                Transaction.Comment = CommentBox.Text;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните все поля корректно.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
