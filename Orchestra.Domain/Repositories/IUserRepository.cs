using Orchestra.Models;
using System.Threading.Tasks;

namespace Orchestra.Infrastructure.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string email, string userName);
        Task<bool> AnyAsync();
        Task AddAsync(User user);
        Task<List<User>> GetAllAsync(CancellationToken cancellationToken);
    }
}
