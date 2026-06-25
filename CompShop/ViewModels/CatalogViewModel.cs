using CommunityToolkit.Mvvm.Input;
using CompShop.Models;
using CompShop.Repositories;
using CompShop.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CompShop.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private readonly IComponentService _componentService;
        private List<Component> _allComponents = new();

        private string _searchText = string.Empty;
        private Category? _selectedCategory;
        private decimal? _maxPrice;
        private int _selectedSortIndex = -1;
        private int _itemsCount;
        private Component? _selectedComponent;
        public bool CanAddToRepair => AppState.IsMaster || AppState.IsAdmin;

        public ObservableCollection<Component> Components { get; } = new();
        public ObservableCollection<Category> Categories { get; } = new();

        public CatalogViewModel(IComponentService componentService)
        {
            _componentService = componentService;

            ResetFiltersCommand = new RelayCommand(ExecuteResetFilters);
            AddToRepairCommand = new RelayCommand<Component>(ExecuteAddToRepair);

            _ = LoadDataAsync();
        }

        public Component? SelectedComponent
        {
            get => _selectedComponent;
            set
            {
                if (_selectedComponent != value)
                {
                    _selectedComponent = value;
                    OnPropertyChanged(nameof(SelectedComponent));
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    ApplyFilters();
                }
            }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged(nameof(SelectedCategory));
                    ApplyFilters();
                }
            }
        }

        public decimal? MaxPrice
        {
            get => _maxPrice;
            set
            {
                if (_maxPrice != value)
                {
                    _maxPrice = value;
                    OnPropertyChanged(nameof(MaxPrice));
                    ApplyFilters();
                }
            }
        }

        public int SelectedSortIndex
        {
            get => _selectedSortIndex;
            set
            {
                if (_selectedSortIndex != value)
                {
                    _selectedSortIndex = value;
                    OnPropertyChanged(nameof(SelectedSortIndex));
                    ApplyFilters();
                }
            }
        }

        public int ItemsCount
        {
            get => _itemsCount;
            private set
            {
                if (_itemsCount != value)
                {
                    _itemsCount = value;
                    OnPropertyChanged(nameof(ItemsCount));
                }
            }
        }

        public ICommand ResetFiltersCommand { get; }
        public ICommand AddToRepairCommand { get; }

        public async Task LoadDataAsync()
        {
            try
            {
                _allComponents = await _componentService.GetComponentsAsync();

                using (var db = new CompShopDbContext())
                {
                    var allStocks = await db.StockBalances.ToListAsync();
                    foreach (var component in _allComponents)
                    {
                        int totalQuantity = allStocks
                            .Where(s => s.ComponentId == component.Id)
                            .Sum(s => s.Quantity);

                        component.StockStatus = totalQuantity > 0
                            ? $"В наличии: {totalQuantity} шт."
                            : "Нет на складе";
                    }
                }

                var uniqueCategories = _allComponents
                    .Where(c => c.Category != null)
                    .Select(c => c.Category)
                    .GroupBy(cat => cat.Id)
                    .Select(g => g.First())
                    .OrderBy(cat => cat.CategoryName)
                    .ToList();

                Categories.Clear();
                foreach (var category in uniqueCategories)
                {
                    Categories.Add(category);
                }

                ApplyFilters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка загрузки каталога: {ex.Message}");
            }
        }

        private void ApplyFilters()
        {
            IEnumerable<Component> filtered = _allComponents;

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(c => c.ComponentName != null &&
                    c.ComponentName.Contains(SearchText, StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedCategory != null)
            {
                filtered = filtered.Where(c => c.CategoryId == SelectedCategory.Id);
            }

            if (MaxPrice.HasValue && MaxPrice.Value > 0)
            {
                filtered = filtered.Where(c => c.Price <= MaxPrice.Value);
            }

            if (SelectedSortIndex == 0)
                filtered = filtered.OrderBy(c => c.Price);
            else if (SelectedSortIndex == 1)
                filtered = filtered.OrderByDescending(c => c.Price);

            Components.Clear();
            foreach (var component in filtered)
            {
                Components.Add(component);
            }

            ItemsCount = Components.Count;
        }

        private void ExecuteResetFilters()
        {
            _searchText = string.Empty;
            _selectedCategory = null;
            _maxPrice = null;
            _selectedSortIndex = -1;

            OnPropertyChanged(nameof(SearchText));
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(MaxPrice));
            OnPropertyChanged(nameof(SelectedSortIndex));

            ApplyFilters();
        }

        private async void ExecuteAddToRepair(Component? component)
        {
            if (component == null) return;

            int testRepairId = 1;
            bool success = await _componentService.AddComponentToRepairAsync(component.Id, testRepairId);

            if (success)
            {
                await LoadDataAsync();
            }
        }
    }
}
