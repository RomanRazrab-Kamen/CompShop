using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CompShop.Services
{
    public interface IWindowManagerService
    {
        void OpenWindow<TWindow, TViewModel>() where TWindow : Avalonia.Controls.Window where TViewModel : class;
        void CloseCurrentAndOpen<TWindow, TViewModel>() where TWindow : Avalonia.Controls.Window where TViewModel : class;
    }

    public class WindowManagerService : IWindowManagerService
    {
        private readonly IServiceProvider _serviceProvider;

        public WindowManagerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void OpenWindow<TWindow, TViewModel>()
            where TWindow : Avalonia.Controls.Window
            where TViewModel : class
        {
            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var window = _serviceProvider.GetRequiredService<TWindow>();
                window.DataContext = _serviceProvider.GetRequiredService<TViewModel>();
                window.Show();
            }
        }

        public void CloseCurrentAndOpen<TWindow, TViewModel>()
            where TWindow : Avalonia.Controls.Window
            where TViewModel : class
        {
            if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var newWindow = _serviceProvider.GetRequiredService<TWindow>();
                newWindow.DataContext = _serviceProvider.GetRequiredService<TViewModel>();

                var oldWindow = desktop.MainWindow;

                desktop.MainWindow = newWindow;
                newWindow.Show();

                oldWindow?.Close();
            }
        }
    }
}