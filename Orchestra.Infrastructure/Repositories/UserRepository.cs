using Orchestra.Data.Context;
using Orchestra.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Orchestra.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<User?> GetByIdAsync(string id)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

        public async Task<bool> ExistsAsync(string email, string userName)
            => await _context.Users.AnyAsync(u => u.Email == email || u.UserName == userName);

        public async Task<bool> AnyAsync()
            => await _context.Users.AnyAsync();

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
            => await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}
