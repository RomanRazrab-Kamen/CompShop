using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using CompShop.ViewModels;
using System;
using System.Windows.Input;

namespace CompShop.Windows
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ThemeToggleButton_Click(object? sender, RoutedEventArgs e)
        {
            var app = Application.Current;
            if (app != null)
            {
                app.RequestedThemeVariant = app.RequestedThemeVariant == ThemeVariant.Dark
                    ? ThemeVariant.Light
                    : ThemeVariant.Dark;
            }
        }
    }
}
