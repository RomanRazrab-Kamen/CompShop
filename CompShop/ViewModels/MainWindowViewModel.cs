using System;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using CompShop.Models;
using CompShop.Services;

namespace CompShop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public INavigationService Navigation { get; }

        public ICommand NavigateToCatalogCommand { get; }
        public ICommand NavigateToRepairsCommand { get; }
        public ICommand AddComponentCommand { get; }
        public ICommand EditComponentCommand { get; }
        public ICommand DeleteComponentCommand { get; }
        public ICommand OpenReportCommand { get; }

        public bool CanModifyCatalog => AppState.IsAdmin || AppState.IsManager;
        public bool CanViewReports => AppState.IsAdmin;
        public bool CanAddToRepair => AppState.IsMaster || AppState.IsAdmin;

        public MainWindowViewModel(INavigationService navigationService)
        {
            Navigation = navigationService;

            NavigateToCatalogCommand = new RelayCommand(() => Navigation.NavigateTo<CompShop.Views.CatalogView, CatalogViewModel>());
            NavigateToRepairsCommand = new RelayCommand(() => Navigation.NavigateTo<CompShop.Views.RepairsView, RepairsViewModel>());

            AddComponentCommand = new RelayCommand(ExecuteAddComponent);
            EditComponentCommand = new RelayCommand(ExecuteEditComponent);
            DeleteComponentCommand = new RelayCommand(ExecuteDeleteComponent);
            OpenReportCommand = new RelayCommand(ExecuteOpenReport);

            //Navigation.NavigateTo<CompShop.Views.CatalogView, CatalogViewModel>();
        }

        private void ExecuteOpenReport()
        {
            Navigation.NavigateTo<CompShop.Views.ReportView, ReportViewModel>();
            System.Diagnostics.Debug.WriteLine("[CompShop] Переключение на Аналитический отчет по складам");
        }

        private async void ExecuteAddComponent()
        {
            var lifetime = Avalonia.Application.Current?.ApplicationLifetime
                as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
            var mainWindow = lifetime?.MainWindow;

            if (mainWindow == null) return;

            var dialog = new CompShop.Windows.AddComponentWindow();
            var dialogViewModel = new AddComponentViewModel(dialog);
            dialog.DataContext = dialogViewModel;

            var result = await dialog.ShowDialog<bool>(mainWindow);

            if (result && Navigation.CurrentView is Avalonia.Controls.Control currentView
                       && currentView.DataContext is CatalogViewModel catalogPage)
            {
                await catalogPage.LoadDataAsync();
            }
        }

        private async void ExecuteEditComponent()
        {
            if (Navigation.CurrentView is Avalonia.Controls.Control currentView
                && currentView.DataContext is CatalogViewModel catalogPage)
            {
                var selected = catalogPage.SelectedComponent;
                if (selected == null) return;

                var lifetime = Avalonia.Application.Current?.ApplicationLifetime
                    as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
                var mainWindow = lifetime?.MainWindow;

                if (mainWindow == null) return;

                var dialog = new CompShop.Windows.EditComponentWindow();
                var dialogViewModel = new EditComponentViewModel(dialog, selected);
                dialog.DataContext = dialogViewModel;

                var result = await dialog.ShowDialog<bool>(mainWindow);

                if (result)
                {
                    catalogPage.SelectedComponent = null;
                    await catalogPage.LoadDataAsync();
                }
            }
        }

        private async void ExecuteDeleteComponent()
        {
            if (Navigation.CurrentView is Avalonia.Controls.Control currentView
                && currentView.DataContext is CatalogViewModel catalogPage)
            {
                var selected = catalogPage.SelectedComponent;
                if (selected == null)
                {
                    return;
                }

                try
                {
                    using (var db = new CompShopDbContext())
                    {
                        using var transaction = await db.Database.BeginTransactionAsync();
                        try
                        {
                            var stocks = db.StockBalances.Where(s => s.ComponentId == selected.Id);
                            db.StockBalances.RemoveRange(stocks);

                            var repairDetails = db.RepairComponentDetails.Where(r => r.ComponentId == selected.Id);
                            db.RepairComponentDetails.RemoveRange(repairDetails);

                            var transactionDetails = db.TransactionDetails.Where(t => t.ComponentId == selected.Id);
                            db.TransactionDetails.RemoveRange(transactionDetails);

                            var entityToDelete = await db.Components.FindAsync(selected.Id);
                            if (entityToDelete != null)
                            {
                                db.Components.Remove(entityToDelete);
                            }

                            await db.SaveChangesAsync();
                            await transaction.CommitAsync();

                        }
                        catch (Exception dbEx)
                        {
                            await transaction.RollbackAsync();
                            return;
                        }
                    }

                    catalogPage.SelectedComponent = null;
                    await catalogPage.LoadDataAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompShop] Критическая ошибка при удалении: {ex.Message}");
                }
            }
        }
    }
}
