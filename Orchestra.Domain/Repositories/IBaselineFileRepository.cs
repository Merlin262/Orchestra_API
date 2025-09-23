using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orchestra.Models;

namespace Orchestra.Domain.Repositories
{
    public interface IBaselineFileRepository
    {
        Task<BaselineFile> GetBaselineFileAsync(Guid id);
        Task<List<BaselineFile>> GetBaselineFilesByBaselineIdAsync(int baselineId);
    }
}
