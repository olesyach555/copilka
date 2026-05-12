using System.Windows;
using Kopilka.Shared;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class GoalEditWindow : Window
    {
        public FinancialGoal Goal { get; private set; }

        public GoalEditWindow(FinancialGoal goal)
        {
            InitializeComponent();
            Goal = goal;

            NameBox.Text = goal.Name;
            TargetBox.Text = goal.TargetAmount.ToString();
            TargetDatePicker.SelectedDate = goal.TargetDate == default ? DateTime.Now.AddYears(1) : goal.TargetDate;
            IsFamilyCheck.IsChecked = goal.IsFamilyGoal;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NameBox.Text) &&
                decimal.TryParse(TargetBox.Text, out decimal target))
            {
                Goal.Name = NameBox.Text;
                Goal.TargetAmount = target;
                Goal.TargetDate = TargetDatePicker.SelectedDate ?? DateTime.Now.AddYears(1);
                Goal.IsFamilyGoal = IsFamilyCheck.IsChecked ?? false;

                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните поля корректно.");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
