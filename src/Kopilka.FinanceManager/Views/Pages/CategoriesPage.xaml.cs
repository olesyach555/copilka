using Kopilka.BusinessLogic;
using Kopilka.Shared;
using System.Windows;
using System.Windows.Controls;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class CategoriesPage : Page
    {
        private readonly CategoryService? _categoryService;
        private readonly User? _currentUser;

        public CategoriesPage(User? currentUser, CategoryService categoryService)
        {
            InitializeComponent();
            _currentUser = currentUser;
            _categoryService = categoryService;
            Loaded += CategoriesPage_Loaded;
        }

        private void CategoriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCategories();
        }

        private async void LoadCategories()
        {
            if (_currentUser == null || _categoryService == null)
            {
                CategoriesListView.ItemsSource = null;
                return;
            }
            var categories = await _categoryService.GetCategoriesAsync(_currentUser.Id);
            CategoriesListView.ItemsSource = categories;
        }

        private void AddCategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null || _categoryService == null)
            {
                MessageBox.Show("Для добавления категории необходимо войти в систему.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            var addCategoryWindow = new AddEditCategoryWindow(_categoryService, _currentUser.Id);
            addCategoryWindow.Owner = Window.GetWindow(this);
            if (addCategoryWindow.ShowDialog() == true)
            {
                LoadCategories();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null || _categoryService == null) return;
            if (sender is Button button && button.DataContext is Category category)
            {
                var editCategoryWindow = new AddEditCategoryWindow(_categoryService, _currentUser.Id, category);
                editCategoryWindow.Owner = Window.GetWindow(this);
                if (editCategoryWindow.ShowDialog() == true)
                {
                    LoadCategories();
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Category category && _categoryService != null)
            {
                var result = MessageBox.Show($"Вы уверены, что хотите удалить категорию '{category.Name}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    await _categoryService.DeleteCategoryAsync(category.Id);
                    LoadCategories();
                }
            }
        }
    }
}
