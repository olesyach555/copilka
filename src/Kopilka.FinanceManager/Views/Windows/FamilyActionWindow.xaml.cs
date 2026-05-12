using System.Windows;

namespace Kopilka.FinanceManager.Views.Windows
{
    public partial class FamilyActionWindow : Window
    {
        public string FamilyName => FamilyNameBox.Text;
        public string Password => PasswordBox.Text;
        public bool IsCreate { get; private set; }

        public FamilyActionWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(FamilyName) && !string.IsNullOrWhiteSpace(Password))
            {
                IsCreate = true;
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("Заполните название и пароль.");
        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(Password))
            {
                IsCreate = false;
                DialogResult = true;
                Close();
            }
            else MessageBox.Show("Введите домашний пароль семьи.");
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
