using System.Windows;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class PaymentAddWindow : Window
    {
        public decimal Amount { get; private set; }

        public PaymentAddWindow()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(AmountBox.Text, out decimal amount))
            {
                Amount = amount;
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("Введите корректную сумму.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
