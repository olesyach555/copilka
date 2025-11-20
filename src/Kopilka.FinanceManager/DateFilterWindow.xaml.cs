using System;
using System.Windows;

namespace Kopilka.FinanceManager
{
    /// <summary>
    /// Логика взаимодействия для DateFilterWindow.xaml
    /// </summary>
    public partial class DateFilterWindow : Window
    {
        /// <summary>
        /// Начальная дата, выбранная в окне.
        /// </summary>
        public DateTime? StartDate { get; private set; }

        /// <summary>
        /// Конечная дата, выбранная в окне.
        /// </summary>
        public DateTime? EndDate { get; private set; }

        public DateFilterWindow()
        {
            InitializeComponent();
        }

        private void ApplyButton_Click(object sender, RoutedEventArgs e)
        {
            StartDate = StartDatePicker.SelectedDate;
            EndDate = EndDatePicker.SelectedDate;
            DialogResult = true;
        }
    }
}
