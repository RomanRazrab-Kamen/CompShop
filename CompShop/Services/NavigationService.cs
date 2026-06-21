using System;
using CompShop.ViewModels;

namespace CompShop.Services
{
    public interface INavigationService
    {
        ViewModelBase? CurrentView { get; }
        event Action? CurrentViewChanged;
        void NavigateTo<TViewModel>() where TViewModel : ViewModelBase;
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private ViewModelBase? _currentView;

        public ViewModelBase? CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                CurrentViewChanged?.Invoke(); // Уведомляем окно о смене экрана
            }
        }
        public event Action? CurrentViewChanged;

        // Конструктор получает доступ к DI чтобы создавать любые ViewModel
        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Метод для переключения на любой экран
        public void NavigateTo<TViewModel>() where TViewModel : ViewModelBase
        {
            // Достаем нужную ViewModel из DI-контейнера со всеми её зависимостями и БД
            CurrentView = (ViewModelBase)_serviceProvider.GetService(typeof(TViewModel))!;
        }
    }
}
