using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class DebtsViewModel : ViewModelBase
    {
        private readonly DebtService _debtService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<DebtContract> _debts = new();

        public DebtsViewModel(DebtService debtService, int userId)
        {
            _debtService = debtService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadDebtsAsync()
        {
            var list = await _debtService.GetUserDebtsAsync(_userId);
            Debts = new ObservableCollection<DebtContract>(list);
        }
    }
}
