using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CompShop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentPage = null!;

        public ViewModelBase CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }

        public MainWindowViewModel(CatalogViewModel catalogPage)
        {
            CurrentPage = catalogPage;
        }
        public void NavigateTo(ViewModelBase newPage)
        {
            CurrentPage = newPage;
        }
    }
}
