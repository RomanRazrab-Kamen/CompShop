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
        public ICommand ExportReceiptCommand { get; }

        public RepairsViewModel()
        {
            ExportReceiptCommand = new RepairsRelayCommand(ExecuteExportReceipt);
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
                    var dataFromDb = await db.Repairs
                        .AsNoTracking()
                        .Include(r => r.Manager)
                        .Include(r => r.Master)
                        .OrderByDescending(r => r.DateReceived)
                        .ToListAsync();

                    _allRepairs = dataFromDb ?? new List<Repair>();
                }

                ApplyFilter();

                System.Diagnostics.Debug.WriteLine($"[CompShop] Из БД успешно загружено ремонтов: {_allRepairs.Count} шт.");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка загрузки ремонтов: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompShop] Причина из СУБД: {ex.InnerException.Message}");
                }
            }
        }


        private void ApplyFilter()
        {
            if (_allRepairs == null || !_allRepairs.Any())
            {
                Repairs.Clear();
                return;
            }

            IEnumerable<Repair> filtered = _allRepairs;

            if (SelectedStatusIndex == 1) // В работе
            {
                filtered = filtered.Where(r => r.Status != null &&
                    r.Status.StatusName.Trim().Equals("В работе", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedStatusIndex == 2) // Готов
            {
                filtered = filtered.Where(r => r.Status != null &&
                    r.Status.StatusName.Trim().Equals("Готов", StringComparison.OrdinalIgnoreCase));
            }

            Repairs.Clear();
            foreach (var repair in filtered)
            {
                Repairs.Add(repair);
            }

            System.Diagnostics.Debug.WriteLine($"[CompShop] Фильтр применен. Индекс ComboBox: {SelectedStatusIndex}. На экран отправлено строк: {Repairs.Count} из {_allRepairs.Count}");

            if (Repairs.Count == 0 && _allRepairs.Count > 0)
            {
                var firstRepair = _allRepairs.First();
                System.Diagnostics.Debug.WriteLine($"[CompShop] ВНИМАНИЕ: Запись скрыта фильтром! ID={firstRepair.Id}, Текущий статус в БД = '{firstRepair.Status}'");
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
        private void ExecuteExportReceipt()
        {
            if (SelectedRepair == null || !SelectedRepairComponents.Any()) return;

            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string fileName = $"Receipt_Order_{SelectedRepair.Id}.txt";
                string fullPath = System.IO.Path.Combine(desktopPath, fileName);

                using (var writer = new System.IO.StreamWriter(fullPath, false, System.Text.Encoding.UTF8))
                {
                    writer.WriteLine("==========================================");
                    writer.WriteLine("          СЕРВИСНЫЙ ЦЕНТР COMPSHOP        ");
                    writer.WriteLine("==========================================");
                    writer.WriteLine($" КВИТАНЦИЯ К ЗАКАЗУ НА РЕМОНТ № {SelectedRepair.Id}");
                    writer.WriteLine($" Дата печати: {DateTime.Now:dd.MM.yyyy HH:mm}");
                    writer.WriteLine($" Устройство:  {SelectedRepair.DeviceName}");
                    writer.WriteLine("------------------------------------------");
                    writer.WriteLine(" НАИМЕНОВАНИЕ         КОЛ-ВО      СТОИМОСТЬ");
                    writer.WriteLine("------------------------------------------");

                    decimal totalComponentsCost = 0;
                    foreach (var detail in SelectedRepairComponents)
                    {
                        string name = detail.Component?.ComponentName ?? "Неизвестная деталь";
                        if (name.Length > 20) name = name.Substring(0, 17) + "...";

                        decimal cost = detail.Quantity * detail.FixedPrice;
                        totalComponentsCost += cost;

                        writer.WriteLine($"{name.PadRight(20)} {detail.Quantity.ToString().PadRight(6)} {cost:N0} ₽");
                    }

                    writer.WriteLine("------------------------------------------");
                    writer.WriteLine($" ИТОГО ЗА ДЕТАЛИ:             {totalComponentsCost:N0} ₽");
                    writer.WriteLine($" СТОИМОСТЬ ЗАКАЗА (ОБЩАЯ):     {SelectedRepair.TotalCost:N0} ₽");
                    writer.WriteLine("==========================================");
                }

                var psi = new System.Diagnostics.ProcessStartInfo(fullPath) { UseShellExecute = true };
                System.Diagnostics.Process.Start(psi);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка печати чека: {ex.Message}");
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