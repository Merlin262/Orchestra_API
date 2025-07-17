﻿using Orchestra.Models;

namespace Orchestra.Repoitories.Interfaces
{
    public interface IBpmnProcessBaselineRepository : IGenericRepository<BpmnProcessBaseline>
    {
        Task<BpmnProcessBaseline?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<BpmnProcessBaseline>> GetAllAsync(CancellationToken cancellationToken);
    }
}
