using Orchestra.Infrastructure.Repositories;
using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Serviecs.Intefaces;

namespace Orchestra.Serviecs
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetByIdAsync(userId, cancellationToken);
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllAsync(cancellationToken);
        }

        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _userRepository.AddAsync(user);
        }
    }
}
