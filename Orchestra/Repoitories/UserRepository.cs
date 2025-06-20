using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users.ToListAsync(cancellationToken);
        }
    }
}
