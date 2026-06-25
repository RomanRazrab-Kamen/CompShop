using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CompShop.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CompShop.ViewModels
{
    public class EditComponentViewModel : ViewModelBase
    {
        private readonly Window _window;
        private readonly int _componentId;
        private string _componentName = string.Empty;
        private decimal? _price;
        private Category? _selectedCategory;

        public ObservableCollection<Category> Categories { get; } = new();
        public ICommand SaveCommand { get; }

        public EditComponentViewModel(Window window, Component componentToEdit)
        {
            _window = window;
            _componentId = componentToEdit.Id;
            _componentName = componentToEdit.ComponentName;
            _price = componentToEdit.Price;

            SaveCommand = new RelayCommand(ExecuteSave);

            using (var db = new CompShopDbContext())
            {
                var list = db.Categories.OrderBy(c => c.CategoryName).ToList();
                foreach (var category in list)
                {
                    Categories.Add(category);
                }

                SelectedCategory = Categories.FirstOrDefault(c => c.Id == componentToEdit.CategoryId);
            }
        }

        public string ComponentName
        {
            get => _componentName;
            set { _componentName = value; OnPropertyChanged(nameof(ComponentName)); }
        }

        public decimal? Price
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
            if (string.IsNullOrWhiteSpace(ComponentName) || SelectedCategory == null || !Price.HasValue || Price.Value <= 0)
            {
                System.Diagnostics.Debug.WriteLine("[CompShop] Валидация не прошла.");
                return;
            }

            try
            {
                using (var db = new CompShopDbContext())
                {
                    var dbComponent = db.Components.FirstOrDefault(c => c.Id == _componentId);

                    if (dbComponent != null)
                    {
                        dbComponent.ComponentName = this.ComponentName;
                        dbComponent.Price = this.Price.Value;

                        dbComponent.CategoryId = this.SelectedCategory.Id;

                        db.Entry(dbComponent).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                        db.SaveChanges();
                        System.Diagnostics.Debug.WriteLine($"[CompShop] Компонент ID {_componentId} успешно обновлен.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка: Компонент с ID {_componentId} не найден в БД.");
                    }
                }

                _window.Close(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Критическая ошибка изменения: {ex.Message}");
            }
        }
    }
}
