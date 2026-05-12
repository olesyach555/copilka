using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;
using System.Windows;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class FamilySettingsViewModel : ViewModelBase
    {
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly int _currentUserId;

        [ObservableProperty]
        private string _familyName = string.Empty;

        [ObservableProperty]
        private string _homePassword = string.Empty;

        [ObservableProperty]
        private ObservableCollection<User> _familyMembers = new();

        [ObservableProperty]
        private bool _hasFamily;

        public Func<Tuple<string, string, bool>?>? ShowFamilyActionDialog;
        public Action<string, string>? ShowErrorDialog;

        public FamilySettingsViewModel(AuthService authService, UserService userService, int userId)
        {
            _authService = authService;
            _userService = userService;
            _currentUserId = userId;
            _ = LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            var user = await _authService.GetUserByIdAsync(_currentUserId);
            if (user != null)
            {
                await LoadFamilyDataAsync(user);
            }
        }

        public async Task LoadFamilyDataAsync(User user)
        {
            if (user.FamilyId.HasValue)
            {
                HasFamily = true;
                var family = await _userService.GetFamilyByIdAsync(user.FamilyId.Value);
                if (family != null)
                {
                    FamilyName = family.Name;
                    HomePassword = family.HomePassword;
                    var members = await _userService.GetFamilyMembersAsync(family.Id);
                    FamilyMembers = new ObservableCollection<User>(members);
                }
            }
            else
            {
                HasFamily = false;
                FamilyName = string.Empty;
                HomePassword = string.Empty;
                FamilyMembers.Clear();
            }
        }

        [RelayCommand]
        public async Task ManageFamilyAsync()
        {
            var result = ShowFamilyActionDialog?.Invoke();
            if (result != null)
            {
                if (result.Item3) // Create
                {
                    await _authService.CreateFamilyAsync(_currentUserId, result.Item1, result.Item2);
                }
                else // Join
                {
                    bool success = await _authService.JoinFamilyByNameAsync(_currentUserId, result.Item1, result.Item2);
                    if (!success)
                    {
                        ShowErrorDialog?.Invoke("Не удалось присоединиться. Проверьте название семьи и пароль.", "Ошибка");
                    }
                }
                await LoadDataAsync();
            }
        }

        [RelayCommand]
        public async Task LeaveFamilyAsync()
        {
            await _userService.RemoveFromFamilyAsync(_currentUserId);
            await LoadDataAsync();
        }
    }
}
