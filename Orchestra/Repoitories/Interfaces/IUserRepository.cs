using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
