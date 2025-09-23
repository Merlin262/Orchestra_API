using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Orchestra.Models;
using Orchestra.Domain.Repositories;

namespace Orchestra.Services.Serviecs
{
    public class BaselineFileService : IBaselineFileService
    {
        private readonly IBaselineFileRepository _repository;
        public BaselineFileService(IBaselineFileRepository repository)
        {
            _repository = repository;
        }

        public async Task<BaselineFile> GetFileContentAsync(Guid id)
        {
            return await _repository.GetBaselineFileAsync(id);
        }

        public async Task<List<BaselineFile>> GetFilesByBaselineIdAsync(int baselineId)
        {
            return await _repository.GetBaselineFilesByBaselineIdAsync(baselineId);
        }
    }
}
