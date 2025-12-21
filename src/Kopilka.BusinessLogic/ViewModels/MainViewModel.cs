using Kopilka.BusinessLogic.ViewModels.Base;
using Kopilka.Shared;

namespace Kopilka.BusinessLogic.ViewModels
{
    /// <summary>
    /// Главная ViewModel, управляющая основным состоянием приложения.
    /// </summary>
    public partial class MainViewModel : ViewModelBase
    {
        private User? _currentUser;

        /// <summary>
        /// Текущий аутентифицированный пользователь.
        /// </summary>
        public User? CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public MainViewModel(User currentUser)
        {
            _currentUser = currentUser;
        }
    }
}
