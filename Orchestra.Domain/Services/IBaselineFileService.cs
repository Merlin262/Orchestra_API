using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orchestra.Models;

namespace Orchestra.Services.Serviecs
{
    public interface IBaselineFileService
    {
        Task<BaselineFile> GetFileContentAsync(Guid id);
        Task<List<BaselineFile>> GetFilesByBaselineIdAsync(int baselineId);
    }
}
