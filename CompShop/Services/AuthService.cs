using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CompShop.Models;

namespace CompShop.Services
{
    public interface IAuthService
    {
        Task<Employee?> ValidateUserAsync(string login, string password);
    }

    public class AuthService : IAuthService
    {
        private readonly CompShopDbContext _context;

        public AuthService(CompShopDbContext context)
        {
            _context = context;
        }

        public async Task<Employee?> ValidateUserAsync(string login, string password)
        {
            // Асинхронно ищем сотрудника в БД чтобы интерфейс окна не зависал
            return await _context.Employees
                .FirstOrDefaultAsync(e => e.Login == login && e.Password == password);
        }
    }
}
