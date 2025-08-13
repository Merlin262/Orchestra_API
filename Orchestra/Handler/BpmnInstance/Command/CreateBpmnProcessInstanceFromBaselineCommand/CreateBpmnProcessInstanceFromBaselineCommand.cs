using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceFromBaselineCommand
{
    public class CreateBpmnProcessInstanceFromBaselineCommand : IRequest<BpmnProcessInstance>
    {
        public int BaselineId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string UserId { get; set; } 
        public CreateBpmnProcessInstanceFromBaselineCommand(int baselineId, string? name, string? description, string userId)
        {
            BaselineId = baselineId;
            Name = name;
            Description = description;
            UserId = userId;
        }
    }
}
