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
    }
}
