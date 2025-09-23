using Orchestra.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Domain.Repositories
{
    public interface IBaselineHistoryRepository : IGenericRepository<BaselineHistory>
    {
        Task AddAsync(BaselineHistory history, CancellationToken cancellationToken);
    }
}
