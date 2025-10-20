using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Orchestra.Domain.Models;

namespace Orchestra.Domain.Repositories
{
    public interface ISubProcessRepository
    {
        Task<SubProcess> AddAsync(SubProcess subProcess, CancellationToken cancellationToken = default);
        Task<List<SubProcess>> GetByBaselineIdAsync(int processBaselineId, CancellationToken cancellationToken = default);
        Task<List<SubProcess>> GetByBaselineHistoryIdAsync(int baselineHistoryId, CancellationToken cancellationToken = default);
        Task UpdateAsync(SubProcess subProcess, CancellationToken cancellationToken = default);
        Task DeleteAsync(SubProcess subProcess, CancellationToken cancellationToken = default);
    }
}
