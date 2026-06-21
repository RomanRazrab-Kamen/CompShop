using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CompShop.Models;
using CompShop.Repositories;

namespace CompShop.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private readonly IComponentService _componentRepository;

        public string SearchText { get; set; } = string.Empty;
        public ObservableCollection<string> Categories { get; set; } = new();
        public string SelectedCategory { get; set; } = string.Empty;
        public decimal MaxPrice { get; set; }
        public ICommand? ResetFiltersCommand { get; set; }

        public ObservableCollection<Component> Components { get; set; } = new();
        public int ItemsCount => Components.Count;

        public CatalogViewModel(IComponentService componentRepository)
        {
            _componentRepository = componentRepository;
            LoadComponentsFromDb();
        }

        private async void LoadComponentsFromDb()
        {
            var items = await _componentRepository.GetComponentsAsync();

            Components.Clear();
            foreach (var item in items)
            {
                Components.Add(item);
            }
        }
    }
}
