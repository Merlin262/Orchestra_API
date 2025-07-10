using HotChocolate;
using HotChocolate.Data;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Enums;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories.Interfaces;

namespace Orchestra.GraphQLConfig
{
    public class BpmnProcessInstanceQuery
    {
        // Exemplo com repositório injetado via serviço
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public async Task<IEnumerable<BpmnProcessInstance>> GetBpmnProcessInstances([Service] IBpmnProcessInstanceRepository repository)
        {
            return await repository.GetAllAsync(CancellationToken.None);
        }

        public Task<BpmnProcessInstance?> GetBpmnProcessInstanceById(int id, [Service] IBpmnProcessInstanceRepository repository)
        {
            return repository.GetByIdAsync(id, CancellationToken.None);
        }
    }
    public class CreateBpmnProcessInstanceInput
    {
        public string Name { get; set; } = default!;
        public string XmlContent { get; set; } = default!;
        public int BpmnProcessBaselineId { get; set; }
        public List<string>? PoolNames { get; set; }
        public string Status { get; set; } = default!;
    }

    public class Mutation
    {
        public async Task<BpmnProcessInstance> CreateBpmnProcessInstance(
            CreateBpmnProcessInstanceInput input,
            [Service] IBpmnProcessInstanceRepository repository)
        {
            if (!Enum.TryParse<StatusEnum>(input.Status, true, out var statusEnum))
                throw new ArgumentException($"Status '{input.Status}' não é válido. Use: NotStarted, InProgress ou Finished.");

            var instance = new BpmnProcessInstance
            {
                Name = input.Name,
                XmlContent = input.XmlContent,
                CreatedAt = DateTime.UtcNow,
                BpmnProcessBaselineId = input.BpmnProcessBaselineId,
                PoolNames = input.PoolNames ?? new List<string>(),
                Status = statusEnum
            };

            await repository.AddAsync(instance);
            return instance;
        }
    }

}