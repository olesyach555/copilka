using System.Windows;
using Kopilka.Shared;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class ReminderEditWindow : Window
    {
        public Reminder Reminder { get; private set; }

        public ReminderEditWindow(Reminder reminder, List<Category> categories)
        {
            InitializeComponent();
            Reminder = reminder;

            TitleBox.Text = reminder.Title;
            ReminderDatePicker.SelectedDate = reminder.ReminderDate == default ? DateTime.Now : reminder.ReminderDate;
            IsRecurringCheck.IsChecked = reminder.IsRecurring;

            CategoryCombo.ItemsSource = categories;
            if (reminder.TransactionCategoryId.HasValue)
            {
                CategoryCombo.SelectedItem = categories.FirstOrDefault(c => c.Id == reminder.TransactionCategoryId.Value);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleBox.Text))
            {
                Reminder.Title = TitleBox.Text;
                Reminder.ReminderDate = ReminderDatePicker.SelectedDate ?? DateTime.Now;
                Reminder.IsRecurring = IsRecurringCheck.IsChecked ?? false;

                if (CategoryCombo.SelectedItem is Category category)
                    Reminder.TransactionCategoryId = category.Id;
                else
                    Reminder.TransactionCategoryId = null;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите заголовок.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
