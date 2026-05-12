using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Kopilka.Shared;

namespace Kopilka.BusinessLogic.ViewModels
{
    public partial class AuthViewModel : ViewModelBase
    {
        private readonly AuthService _authService;

        [ObservableProperty]
        private string _login = string.Empty;

        [ObservableProperty]
        private string _password = string.Empty;

        [ObservableProperty]
        private string? _errorMessage;

        public event Action<User>? OnAuthenticated;

        public AuthViewModel(AuthService authService)
        {
            _authService = authService;
        }

        [RelayCommand]
        public async Task LoginAsync()
        {
            ErrorMessage = null;
            var user = await _authService.LoginAsync(Login, Password);
            if (user != null)
            {
                OnAuthenticated?.Invoke(user);
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль";
            }
        }

        [RelayCommand]
        public async Task RegisterAsync()
        {
            ErrorMessage = null;
            if (string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Введите логин и пароль";
                return;
            }

            var user = await _authService.RegisterUserAsync(Login, Password);
            if (user != null)
            {
                OnAuthenticated?.Invoke(user);
            }
            else
            {
                ErrorMessage = "Пользователь с таким логином уже существует";
            }
        }
    }
}
