using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager
{
    public partial class AddEditCategoryWindow : Window
    {
        private readonly CategoryService _categoryService;
        private readonly int _userId;
        private Category? _categoryToEdit; // Поле теперь nullable

        public AddEditCategoryWindow(CategoryService categoryService, int userId, Category? categoryToEdit = null)
        {
            InitializeComponent();
            _categoryService = categoryService;
            _userId = userId;
            _categoryToEdit = categoryToEdit;

            if (_categoryToEdit != null)
            {
                WindowTitle.Text = "Редактировать категорию";
                CategoryNameTextBox.Text = _categoryToEdit.Name;
                CategoryTypeComboBox.SelectedItem = _categoryToEdit.Type == "Income" ? "Доход" : "Расход";
            }
            else
            {
                CategoryTypeComboBox.SelectedIndex = 0; // "Доход" по умолчанию
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Название категории не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var categoryType = (CategoryTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() == "Доход" ? "Income" : "Expense";

            if (_categoryToEdit == null)
            {
                var newCategory = new Category
                {
                    UserId = _userId,
                    Name = CategoryNameTextBox.Text,
                    Type = categoryType
                };
                await _categoryService.AddCategoryAsync(newCategory);
            }
            else
            {
                _categoryToEdit.Name = CategoryNameTextBox.Text;
                _categoryToEdit.Type = categoryType;
                await _categoryService.UpdateCategoryAsync(_categoryToEdit);
            }

            DialogResult = true;
            Close();
        }
    }
}
