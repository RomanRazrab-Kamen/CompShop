using System;
using System.Collections.Generic;
using System.Text;
using CompShop.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CompShop.Repositories
{
    public interface IComponentService
    {
        Task<List<Component>> GetComponentsAsync();
    }
    public class ComponentRepository : IComponentService
    {
        private readonly CompShopDbContext _compShopDbContext;
        public ComponentRepository(CompShopDbContext compShopDbContext) => _compShopDbContext = compShopDbContext;

        public async Task<List<Component>> GetComponentsAsync() =>
            await _compShopDbContext.Components.ToListAsync();
    }
}
