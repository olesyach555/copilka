using System.Windows;
using Kopilka.Shared;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class DebtEditWindow : Window
    {
        public DebtContract Debt { get; private set; }

        public DebtEditWindow(DebtContract debt)
        {
            InitializeComponent();
            Debt = debt;

            TitleBox.Text = debt.Title;
            PrincipalBox.Text = debt.Principal.ToString();
            RateBox.Text = debt.InterestRate.ToString();
            CounterpartyBox.Text = debt.Counterparty;
            StartDatePicker.SelectedDate = debt.StartDate == default ? DateTime.Now : debt.StartDate;
            EndDatePicker.SelectedDate = debt.EndDate;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TitleBox.Text) &&
                decimal.TryParse(PrincipalBox.Text, out decimal principal) &&
                decimal.TryParse(RateBox.Text, out decimal rate))
            {
                Debt.Title = TitleBox.Text;
                Debt.Principal = principal;
                Debt.InterestRate = rate;
                Debt.Counterparty = CounterpartyBox.Text;
                Debt.StartDate = StartDatePicker.SelectedDate ?? DateTime.Now;
                Debt.EndDate = EndDatePicker.SelectedDate;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните основные поля корректно.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
