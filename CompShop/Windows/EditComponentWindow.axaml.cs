using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CompShop.Windows
{
    public partial class EditComponentWindow : Window
    {
        public EditComponentWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
