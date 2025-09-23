using Orchestra.Models;

namespace Orchestra.Serviecs.Intefaces
{
    public interface IUserService
    {
        Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddAsync(User user, CancellationToken cancellationToken = default);
    }
}