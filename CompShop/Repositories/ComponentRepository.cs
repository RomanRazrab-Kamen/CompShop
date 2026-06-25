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

                if (component == null) return false;

                var stock = await _compShopDbContext.StockBalances
                    .FirstOrDefaultAsync(s => s.ComponentId == componentId && s.Quantity > 0);

                if (stock == null)
                {
                    return false;
                }

                stock.Quantity -= 1;

                var existingDetail = await _compShopDbContext.RepairComponentDetails
                    .FirstOrDefaultAsync(d => d.RepairId == repairId && d.ComponentId == componentId);

                if (existingDetail != null)
                {
                    existingDetail.Quantity += 1;
                }
                else
                {
                    var newDetail = new RepairComponentDetail
                    {
                        RepairId = repairId,
                        ComponentId = componentId,
                        Quantity = 1,
                        FixedPrice = component.Price
                    };
                    await _compShopDbContext.RepairComponentDetails.AddAsync(newDetail);
                }

                await _compShopDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                System.Diagnostics.Debug.WriteLine($"Ошибка транзакции списания: {ex.Message}");
                return false;
            }
        }
    }
}
