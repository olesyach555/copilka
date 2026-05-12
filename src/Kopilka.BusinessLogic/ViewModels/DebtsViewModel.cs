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

        [ObservableProperty]
        private DebtContract? _selectedDebt;

        [ObservableProperty]
        private ObservableCollection<PaymentSchedule> _schedule = new();

        public Func<DebtContract, bool?>? ShowEditDialog;
        public Func<string, string, bool>? ShowConfirmDialog;

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

        partial void OnSelectedDebtChanged(DebtContract? value)
        {
            _ = LoadScheduleAsync();
        }

        public async Task LoadScheduleAsync()
        {
            if (SelectedDebt != null)
            {
                var list = await _debtService.GetScheduleAsync(SelectedDebt.Id);
                Schedule = new ObservableCollection<PaymentSchedule>(list);
            }
            else
            {
                Schedule.Clear();
            }
        }

        [RelayCommand]
        public async Task AddDebtAsync()
        {
            var debt = new DebtContract { UserId = _userId, StartDate = DateTime.Now };
            if (ShowEditDialog?.Invoke(debt) == true)
            {
                await _debtService.AddDebtAsync(debt);
                await LoadDebtsAsync();
            }
        }

        [RelayCommand]
        public async Task EditDebtAsync(DebtContract debt)
        {
            if (debt == null) return;
            if (ShowEditDialog?.Invoke(debt) == true)
            {
                await _debtService.UpdateDebtAsync(debt);
                await LoadDebtsAsync();
            }
        }

        [RelayCommand]
        public async Task DeleteDebtAsync(DebtContract debt)
        {
            if (debt == null) return;
            if (ShowConfirmDialog?.Invoke("Удалить этот долг?", "Удаление") == true)
            {
                await _debtService.DeleteDebtAsync(debt.Id);
                await LoadDebtsAsync();
            }
        }

        [RelayCommand]
        public async Task MarkPaidAsync(PaymentSchedule item)
        {
            if (item == null || item.IsPaid) return;
            await _debtService.MarkAsPaidAsync(item.Id);
            await LoadScheduleAsync();
        }
    }
}
