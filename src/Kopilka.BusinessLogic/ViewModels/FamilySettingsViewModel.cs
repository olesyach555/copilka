using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;
using System.Collections.ObjectModel;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class FamilySettingsViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _familyName = string.Empty;

        [ObservableProperty]
        private string _homePassword = string.Empty;

        [ObservableProperty]
        private ObservableCollection<User> _familyMembers = new();

        public FamilySettingsViewModel()
        {
            // В реальном приложении здесь была бы загрузка данных о семье через сервис
        }

        [RelayCommand]
        private void SaveSettings()
        {
            // Сохранение настроек
        }
    }
}
