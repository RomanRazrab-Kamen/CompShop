using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

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
            var currentTheme = Application.Current!.RequestedThemeVariant;
            Application.Current.RequestedThemeVariant =
                currentTheme == ThemeVariant.Dark ? ThemeVariant.Light : ThemeVariant.Dark;
        }
    }
}