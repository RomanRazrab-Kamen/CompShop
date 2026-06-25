using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using CompShop.Models;

namespace CompShop.ViewModels
{
    public class RepairsViewModel : ViewModelBase
    {
        private List<Repair> _allRepairs = new();
        private Repair? _selectedRepair;
        private int _selectedStatusIndex = 0;
        private bool _isDetailsVisible;

        public ObservableCollection<Repair> Repairs { get; } = new();
        public ObservableCollection<RepairComponentDetail> SelectedRepairComponents { get; } = new();

        public RepairsViewModel()
        {
            MarkAsReadyCommand = new RepairsRelayCommand(ExecuteMarkAsReady);
            _ = LoadRepairsAsync();
        }
        public Repair? SelectedRepair
        {
            get => _selectedRepair;
            set
            {
                if (_selectedRepair != value)
                {
                    _selectedRepair = value;
                    OnPropertyChanged(nameof(SelectedRepair));
                    _ = LoadRepairDetailsAsync();
                }
            }
        }

        public int SelectedStatusIndex
        {
            get => _selectedStatusIndex;
            set
            {
                if (_selectedStatusIndex != value)
                {
                    _selectedStatusIndex = value;
                    OnPropertyChanged(nameof(SelectedStatusIndex));
                    ApplyFilter();
                }
            }
        }

        public bool IsDetailsVisible
        {
            get => _isDetailsVisible;
            set
            {
                if (_isDetailsVisible != value)
                {
                    _isDetailsVisible = value;
                    OnPropertyChanged(nameof(IsDetailsVisible));
                }
            }
        }

        public ICommand MarkAsReadyCommand { get; }

        public async Task LoadRepairsAsync()
        {
            try
            {
                using (var db = new CompShopDbContext())
                {
                    _allRepairs = await db.Set<Repair>()
                        .AsNoTracking()
                        .Include(r => r.Manager)
                        .Include(r => r.Master)
                        .OrderByDescending(r => r.DateReceived)
                        .ToListAsync();
                }
                ApplyFilter();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка загрузки ремонтов: {ex.Message}");
            }
        }

        private void ApplyFilter()
        {
            IEnumerable<Repair> filtered = _allRepairs;

            if (SelectedStatusIndex == 1)
                filtered = filtered.Where(r => r.Status.StatusName == "В работе");
            else if (SelectedStatusIndex == 2)
                filtered = filtered.Where(r => r.Status.StatusName == "Готов");

            Repairs.Clear();
            foreach (var repair in filtered)
            {
                Repairs.Add(repair);
            }
        }

        private async Task LoadRepairDetailsAsync()
        {
            if (SelectedRepair == null)
            {
                IsDetailsVisible = false;
                SelectedRepairComponents.Clear();
                return;
            }

            try
            {
                using (var db = new CompShopDbContext())
                {
                    var details = await db.RepairComponentDetails
                        .AsNoTracking()
                        .Where(d => d.RepairId == SelectedRepair.Id)
                        .Include(d => d.Component)
                        .ToListAsync();

                    SelectedRepairComponents.Clear();
                    foreach (var detail in details)
                    {
                        SelectedRepairComponents.Add(detail);
                    }

                    IsDetailsVisible = SelectedRepairComponents.Any();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка загрузки состава деталей: {ex.Message}");
            }
        }

        private async void ExecuteMarkAsReady()
        {
            if (SelectedRepair == null) return;

            try
            {
                using (var db = new CompShopDbContext())
                {
                    var dbRepair = await db.Set<Repair>().FindAsync(SelectedRepair.Id);
                    if (dbRepair != null)
                    {
                        dbRepair.Status.StatusName = "Готов";
                        await db.SaveChangesAsync();
                    }
                }

                SelectedRepair = null;
                await LoadRepairsAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка смены статуса: {ex.Message}");
            }
        }
    }

    public class RepairsRelayCommand : ICommand
    {
        private readonly Action _execute;
        public RepairsRelayCommand(Action execute) => _execute = execute;
        public bool CanExecute(object? parameter) => true;
        public void Execute(object? parameter) => _execute();

        public event EventHandler? CanExecuteChanged { add { } remove { } }
    }

}