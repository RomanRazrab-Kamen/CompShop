using Avalonia;
using Microsoft.EntityFrameworkCore;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CompShop.Models;
using CompShop.Views;
using CompShop.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using CompShop.Windows;
using CompShop.Repositories;

namespace CompShop
{
    public partial class App : Application
    {
        public static IServiceProvider? Services { get; private set; }
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var serviceCollection = new ServiceCollection();

            // База данных и репозитории
            serviceCollection.AddDbContext<CompShopDbContext>(options =>
                options.UseSqlServer("Server=ROOM\\SQLEXPRESS;Database=CompShopDB;Trusted_Connection=True;TrustServerCertificate=True"));
            serviceCollection.AddScoped<IComponentService, ComponentRepository>();

            // Сервисы
            serviceCollection.AddSingleton<CompShop.Services.INavigationService, CompShop.Services.NavigationService>();
            serviceCollection.AddScoped<CompShop.Services.IAuthService, CompShop.Services.AuthService>();
            serviceCollection.AddSingleton<CompShop.Services.IWindowManagerService, CompShop.Services.WindowManagerService>();

            // Регистрация Окон и их ViewModel
            serviceCollection.AddTransient<AuthorizationViewModel>();
            serviceCollection.AddTransient<AuthorizationWindow>();

            serviceCollection.AddTransient<MainWindowViewModel>();
            serviceCollection.AddTransient<MainWindow>();

            serviceCollection.AddTransient<CatalogViewModel>();
            serviceCollection.AddTransient<CatalogView>();

            serviceCollection.AddTransient<RepairsViewModel>();
            serviceCollection.AddTransient<RepairsView>();

            serviceCollection.AddTransient<ReportViewModel>();
            serviceCollection.AddTransient<ReportView>();

            Services = serviceCollection.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var authWindow = Services.GetRequiredService<AuthorizationWindow>();
                authWindow.DataContext = Services.GetRequiredService<AuthorizationViewModel>();
                desktop.MainWindow = authWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}