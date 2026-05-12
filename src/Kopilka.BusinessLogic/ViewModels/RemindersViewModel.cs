using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class RemindersViewModel : ViewModelBase
    {
        private readonly ReminderService _reminderService;
        private readonly CategoryService _categoryService;
        private readonly int _userId;

        [ObservableProperty]
        private ObservableCollection<Reminder> _reminders = new();

        public Func<Reminder, List<Category>, bool?>? ShowEditDialog;
        public Func<string, string, bool>? ShowConfirmDialog;

        public RemindersViewModel(ReminderService reminderService, CategoryService categoryService, int userId)
        {
            _reminderService = reminderService;
            _categoryService = categoryService;
            _userId = userId;
        }

        [RelayCommand]
        public async Task LoadRemindersAsync()
        {
            var list = await _reminderService.GetActiveRemindersAsync(_userId);
            Reminders = new ObservableCollection<Reminder>(list);
        }

        [RelayCommand]
        public async Task AddReminderAsync()
        {
            var reminder = new Reminder { UserId = _userId, ReminderDate = DateTime.Now };
            var categories = await _categoryService.GetCategoriesAsync(_userId);
            if (ShowEditDialog?.Invoke(reminder, categories) == true)
            {
                await _reminderService.AddReminderAsync(reminder);
                await LoadRemindersAsync();
            }
        }

        [RelayCommand]
        public async Task EditReminderAsync(Reminder reminder)
        {
            if (reminder == null) return;
            var categories = await _categoryService.GetCategoriesAsync(_userId);
            if (ShowEditDialog?.Invoke(reminder, categories) == true)
            {
                await _reminderService.UpdateReminderAsync(reminder);
                await LoadRemindersAsync();
            }
        }

        [RelayCommand]
        public async Task DeleteReminderAsync(Reminder reminder)
        {
            if (reminder == null) return;
            if (ShowConfirmDialog?.Invoke("Удалить это напоминание?", "Удаление") == true)
            {
                await _reminderService.DeleteReminderAsync(reminder.Id);
                await LoadRemindersAsync();
            }
        }
    }
}
