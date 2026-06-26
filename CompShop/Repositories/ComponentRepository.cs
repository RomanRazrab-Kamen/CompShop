using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompShop.Models;

namespace CompShop.Repositories
{
    public interface IComponentService
    {
        Task<List<Component>> GetComponentsAsync();
        Task<bool> AddComponentToRepairAsync(int componentId, int repairId);
    }
    public class ComponentRepository : IComponentService
    {
        private readonly CompShopDbContext _compShopDbContext;

        public ComponentRepository(CompShopDbContext compShopDbContext)
        {
            _compShopDbContext = compShopDbContext;
        }

        public async Task<List<Component>> GetComponentsAsync() =>
            await _compShopDbContext.Components
                .AsNoTracking()
                .Include(c => c.Category)
                .ToListAsync();

        public async Task<bool> AddComponentToRepairAsync(int componentId, int repairId)
        {
            using var transaction = await _compShopDbContext.Database.BeginTransactionAsync();
            try
            {
                var component = await _compShopDbContext.Components
                    .FirstOrDefaultAsync(c => c.Id == componentId);

                if (component == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка: Компонент с ID {componentId} не найден в БД.");
                    return false;
                }
                var stock = await _compShopDbContext.StockBalances
                    .FirstOrDefaultAsync(s => s.ComponentId == componentId && s.Quantity > 0);

                if (stock == null)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка: Компонента {component.ComponentName} нет в наличии на складах.");
                    return false;
                }
                var repairExists = await _compShopDbContext.Set<Repair>().AnyAsync(r => r.Id == repairId);
                int targetRepairId = repairId;

                if (!repairExists)
                {
                    var firstRepair = await _compShopDbContext.Set<Repair>().FirstOrDefaultAsync();
                    if (firstRepair == null)
                    {
                        System.Diagnostics.Debug.WriteLine("[CompShop] КРИТИЧЕСКАЯ ОШИБКА: В таблице 'Repairs' нет ни одной записи! Создайте хотя бы один ремонт в СУБД.");
                        return false;
                    }
                    targetRepairId = firstRepair.Id;
                }

                stock.Quantity -= 1;

                var existingDetail = await _compShopDbContext.RepairComponentDetails
                    .FirstOrDefaultAsync(d => d.RepairId == targetRepairId && d.ComponentId == componentId);

                if (existingDetail != null)
                {
                    existingDetail.Quantity += 1;
                }
                else
                {
                    var newDetail = new RepairComponentDetail
                    {
                        RepairId = targetRepairId,
                        ComponentId = componentId,
                        Quantity = 1,
                        FixedPrice = Convert.ToDecimal(component.Price)
                    };
                    await _compShopDbContext.RepairComponentDetails.AddAsync(newDetail);
                }

                await _compShopDbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                System.Diagnostics.Debug.WriteLine($"[CompShop] УСПЕХ: Деталь '{component.ComponentName}' добавлена в ремонт №{targetRepairId}, склад уменьшен.");
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                System.Diagnostics.Debug.WriteLine($"[CompShop] Ошибка транзакции списания: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[CompShop] КРИТИЧЕСКАЯ ПРИЧИНА ИЗ MS SQL: {ex.InnerException.Message}");
                }
                return false;
            }
        }
    }
}
