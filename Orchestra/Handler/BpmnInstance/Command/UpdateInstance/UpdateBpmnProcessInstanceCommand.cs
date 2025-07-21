using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Command.UpdateInstance
{
    public class UpdateBpmnProcessInstanceCommand : IRequest<BpmnProcessInstance?>
    {
        public int Id { get; }
        public string? Name { get; }
        public string? Description { get; }

        public UpdateBpmnProcessInstanceCommand(int id, string? name, string? description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }
}
