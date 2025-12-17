using System;
using System.Windows;
using System.Windows.Controls;
using Kopilka.BusinessLogic.ViewModels;
using Kopilka.DataAccess;
using Kopilka.Shared;

namespace Kopilka.FinanceManager.Views.Pages
{
    public partial class HomePage : Page
    {
        private readonly HomePageViewModel _viewModel;

        // Конструктор теперь принимает ViewModel
        public HomePage(HomePageViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            DataContext = _viewModel;
        }

        private async void AddIncomeButton_Click(object sender, RoutedEventArgs e)
        {
            var addTransactionWindow = new AddTransactionWindow(_viewModel._currentUser);
            if (addTransactionWindow.ShowDialog() == true)
            {
                await _viewModel.RefreshDataAsync();
            }
        }

        private async void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            var addTransactionWindow = new AddTransactionWindow(_viewModel._currentUser);
            if (addTransactionWindow.ShowDialog() == true)
            {
                await _viewModel.RefreshDataAsync();
            }
        }
    }
}
