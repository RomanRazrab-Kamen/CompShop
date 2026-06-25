using Avalonia.Controls;
using Avalonia.Interactivity;

namespace CompShop.Windows
{
    public partial class AddComponentWindow : Window
    {
        public AddComponentWindow()
        {
            InitializeComponent();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(false); // Закрываем окно и возвращаем false (сохранение отменено)
        }
    }
}
