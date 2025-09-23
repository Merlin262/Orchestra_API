using Orchestra.Models;
using Orchestra.Repoitories.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestra.Domain.Repositories
{
    public interface IBaselineHistoryRepository : IGenericRepository<BaselineHistory>
    {
        Task<IEnumerable<BaselineHistory>> GetByBaselineIdAsync(int baselineId, CancellationToken cancellationToken);
        Task<IEnumerable<BaselineHistory>> GetAllAsync(CancellationToken cancellationToken);
        Task<BaselineHistory?> GetByIdAsync(int id, CancellationToken cancellationToken);
    }
}
