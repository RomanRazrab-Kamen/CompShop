using Avalonia.Controls;
using System;

namespace CompShop.Services
{
    public interface INavigationService
    {
        Control? CurrentView { get; }
        void NavigateTo<TView, TViewModel>() where TView : Control where TViewModel : class;
    }

    public class NavigationService : CompShop.ViewModels.ViewModelBase, INavigationService
    {
        private Control? _currentView;

        public Control? CurrentView
        {
            get => _currentView;
            private set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public void NavigateTo<TView, TViewModel>() where TView : Control where TViewModel : class
        {
            try
            {
                var view = (App.Services?.GetService(typeof(TView)) ?? Activator.CreateInstance<TView>()) as Control;

                var viewModel = App.Services?.GetService(typeof(TViewModel));

                if (view != null && viewModel != null)
                {
                    view.DataContext = viewModel;
                    CurrentView = view;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[NavigationError] {ex.Message}");
            }
        }
    }
}
