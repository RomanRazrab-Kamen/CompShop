using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using CompShop.Services;
using CompShop.Windows;
using System;
using System.Windows.Input;

namespace CompShop.ViewModels
{
    public class AuthorizationViewModel : ViewModelBase
    {
        private readonly IAuthService _authService;
        private string _loginText = string.Empty;
        private string _passwordText = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isErrorVisible;

        public AuthorizationViewModel(IAuthService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(ExecuteLogin);
        }

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
            set { _errorMessage = value; OnPropertyChanged(nameof(ErrorMessage)); }
        }

        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set { _isErrorVisible = value; OnPropertyChanged(nameof(IsErrorVisible)); }
        }

        public ICommand LoginCommand { get; }

        private async void ExecuteLogin()
        {
            if (string.IsNullOrWhiteSpace(LoginText) || string.IsNullOrWhiteSpace(PasswordText))
            {
                ErrorMessage = "Заполните все поля ввода!";
                IsErrorVisible = true;
                return;
            }

            var employee = await _authService.ValidateUserAsync(LoginText, PasswordText);

            if (employee != null)
            {
                AppState.CurrentUser = employee;
                IsErrorVisible = false;

                var lifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
                if (lifetime != null)
                {
                    var mainWindow = App.Services?.GetService(typeof(MainWindow)) as MainWindow;
                    var mainViewModel = App.Services?.GetService(typeof(MainWindowViewModel)) as MainWindowViewModel;

                    if (mainWindow != null && mainViewModel != null)
                    {
                        mainWindow.DataContext = mainViewModel;

                        var oldAuthWindow = lifetime.MainWindow;
                        lifetime.MainWindow = mainWindow;

                        mainWindow.Show();
                        oldAuthWindow?.Close();
                    }
                }
            }
            else
            {
                ErrorMessage = "Неверный логин или пароль!";
                IsErrorVisible = true;
            }
        }
    }
}
