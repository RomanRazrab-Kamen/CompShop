using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CompShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CompShop.ViewModels
{
    public class AddComponentViewModel : ViewModelBase
    {
        private readonly Window _window;
        private string _componentName = string.Empty;
        private decimal _price;
        private Category? _selectedCategory;

        public ObservableCollection<Category> Categories { get; } = new();

        public ICommand SaveCommand { get; }

        public AddComponentViewModel(Window window)
        {
            _window = window;
            SaveCommand = new RelayCommand(ExecuteSave);

            using (var db = new CompShopDbContext())
            {
                var list = db.Categories.OrderBy(c => c.CategoryName).ToList();
                foreach (var category in list)
                {
                    Categories.Add(category);
                }
            }
        }

        public string ComponentName
        {
            get => _componentName;
            set { _componentName = value; OnPropertyChanged(nameof(ComponentName)); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(nameof(Price)); }
        }

        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set { _selectedCategory = value; OnPropertyChanged(nameof(SelectedCategory)); }
        }

        private void ExecuteSave()
        {
            if (string.IsNullOrWhiteSpace(ComponentName) || SelectedCategory == null || Price <= 0)
            {
                return;
            }

            try
            {
                using (var db = new CompShopDbContext())
                {
                    var newComponent = new Component
                    {
                        ComponentName = this.ComponentName,
                        Price = this.Price,
                        CategoryId = this.SelectedCategory.Id
                    };

                    db.Components.Add(newComponent);
                    db.SaveChanges();
                }

                _window.Close(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Ошибка сохранения детали: {ex.Message}");
            }
        }
    }
}
