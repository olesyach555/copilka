using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class RemindersViewModel : ViewModelBase
    {
        private readonly ReminderService _reminderService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders = new();

        public RemindersViewModel(ReminderService reminderService, int userId)
        {
            _reminderService = reminderService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadRemindersAsync()
        {
            var list = await _reminderService.GetActiveRemindersAsync(_userId);
            Reminders = new ObservableCollection<Reminder>(list);
        }
    }
}
