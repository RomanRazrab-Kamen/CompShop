using CommunityToolkit.Mvvm.Input;
using CompShop.Services;
using CompShop.Windows;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompShop.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private readonly IWindowManagerService _windowManager;

        private string _loginText = string.Empty;
        private string _passwordText = string.Empty;
        private string _errorMessage = string.Empty;

        public string LoginText
        {
            get => _loginText;
            set { _loginText = value; OnPropertyChanged(nameof(LoginText)); }
        }

        public string PasswordText
        {
            get => _passwordText;
            set { _passwordText = value; OnPropertyChanged(nameof(PasswordText)); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(IsErrorVisible));
            }
        }

        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }

        // Конструктор принимает только сервисы! Никаких DbContext и окон внутри.
        public AuthorizationViewModel(IAuthService authService, IWindowManagerService windowManager)
        {
            _authService = authService;
            _windowManager = windowManager;
            LoginCommand = new AsyncRelayCommand(OnLoginExecuteAsync);
        }

        private async Task OnLoginExecuteAsync()
        {
            if (string.IsNullOrWhiteSpace(LoginText) || string.IsNullOrWhiteSpace(PasswordText))
            {
                ErrorMessage = "Пожалуйста, заполните все поля.";
                return;
            }

            try
            {
                // Вызываем изолированный сервис авторизации
                var employee = await _authService.ValidateUserAsync(LoginText, PasswordText);

                if (employee != null)
                {
                    ErrorMessage = string.Empty;

                    // Переключаем окна через специализированный сервис
                    _windowManager.CloseCurrentAndOpen<MainWindow, MainWindowViewModel>();
                }
                else
                {
                    ErrorMessage = "Неверный логин или пароль!";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка сети: {ex.Message}";
            }
        }
    }
}
