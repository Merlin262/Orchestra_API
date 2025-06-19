using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.Repoitories
{
    public class BpmnProcessInstanceRepository : GenericRepository<BpmnProcessInstance>, IBpmnProcessInstanceRepository
    {
        public BpmnProcessInstanceRepository(ApplicationDbContext context) : base(context) { }

        // Aqui você pode adicionar métodos específicos, se necessário
        public async Task<BpmnProcessInstance> AddAsync(BpmnProcessInstance instance, CancellationToken cancellationToken = default)
        {
            await base.AddAsync(instance, cancellationToken);
            return instance;
        }
    }

}
