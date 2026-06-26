using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompShop.Models;

namespace CompShop.ViewModels
{
    public class ReportViewModel : ViewModelBase
    {
        private string _grandTotalText = "Расчет финансовых показателей...";
        public ObservableCollection<WarehouseSummaryRow> WarehouseReports { get; } = new();

        public string GrandTotalText
        {
            get => _grandTotalText;
            set { _grandTotalText = value; OnPropertyChanged(nameof(GrandTotalText)); }
        }

        public ReportViewModel()
        {
            _ = CalculateStockReportAsync();
        }

        public async Task CalculateStockReportAsync()
        {
            try
            {
                using (var db = new CompShopDbContext())
                {
                    var rawBalances = await db.StockBalances
                        .AsNoTracking()
                        .Include(s => s.Warehouse)
                        .Include(s => s.Component)
                        .ToListAsync();

                    if (rawBalances == null || !rawBalances.Any())
                    {
                        GrandTotalText = "На складах компании нет комплектующих.";
                        return;
                    }

                    var calculatedRows = rawBalances
                        .GroupBy(s => s.WarehouseId)
                        .Select(group => new WarehouseSummaryRow
                        {
                            WarehouseName = group.First().Warehouse?.WarehouseName ?? $"Склад №{group.Key}",
                            UniqueItemsCount = group.Select(s => s.ComponentId).Distinct().Count(),
                            TotalQuantity = group.Sum(s => s.Quantity),
                            TotalValue = group.Sum(s => s.Quantity * (s.Component?.Price ?? 0))
                        })
                        .ToList();

                    WarehouseReports.Clear();
                    foreach (var row in calculatedRows)
                    {
                        WarehouseReports.Add(row);
                    }
                    decimal grandTotalMoney = calculatedRows.Sum(w => w.TotalValue);
                    int grandTotalItemsCount = calculatedRows.Sum(w => w.TotalQuantity);

                    GrandTotalText = $"Итого активов: {grandTotalItemsCount} шт. на сумму {grandTotalMoney:N0} ₽";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CompShopReportError] {ex.Message}");
                GrandTotalText = "Ошибка расчета себестоимости.";
            }
        }
    }
    public class WarehouseSummaryRow
    {
        public string WarehouseName { get; set; } = string.Empty;
        public int UniqueItemsCount { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalValue { get; set; }
    }
}
