using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class GoalsViewModel : ViewModelBase
    {
        private readonly GoalService _goalService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<FinancialGoal> _goals = new();

        public Func<FinancialGoal, bool?>? ShowEditDialog;
        public Func<decimal?>? ShowPaymentDialog;
        public Func<string, string, bool>? ShowConfirmDialog;

        public GoalsViewModel(GoalService goalService, int userId)
        {
            _goalService = goalService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadGoalsAsync()
        {
            var list = await _goalService.GetUserGoalsAsync(_userId);
            Goals = new ObservableCollection<FinancialGoal>(list);
        }

        [RelayCommand]
        public async Task AddGoalAsync()
        {
            var goal = new FinancialGoal { OwnerUserId = _userId, TargetDate = DateTime.Now.AddYears(1) };
            if (ShowEditDialog?.Invoke(goal) == true)
            {
                await _goalService.AddGoalAsync(goal);
                await LoadGoalsAsync();
            }
        }

        [RelayCommand]
        public async Task EditGoalAsync(FinancialGoal goal)
        {
            if (goal == null) return;
            if (ShowEditDialog?.Invoke(goal) == true)
            {
                await _goalService.UpdateGoalAsync(goal);
                await LoadGoalsAsync();
            }
        }

        [RelayCommand]
        public async Task DeleteGoalAsync(FinancialGoal goal)
        {
            if (goal == null) return;
            if (ShowConfirmDialog?.Invoke("Удалить эту цель?", "Удаление") == true)
            {
                await _goalService.DeleteGoalAsync(goal.Id);
                await LoadGoalsAsync();
            }
        }

        [RelayCommand]
        public async Task AddFundsAsync(FinancialGoal goal)
        {
            if (goal == null) return;
            var amount = ShowPaymentDialog?.Invoke();
            if (amount.HasValue && amount.Value > 0)
            {
                await _goalService.AddFundsAsync(goal.Id, amount.Value, _userId);
                await LoadGoalsAsync();
            }
        }
    }
}
