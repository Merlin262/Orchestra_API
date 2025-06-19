using MediatR;
using Orchestra.Models.Orchestra.Models;

namespace Orchestra.Handler.BpmnInstance.Command
{
    public class CreateBpmnProcessInstanceFromBaselineCommand : IRequest<BpmnProcessInstance>
    {
        public int BaselineId { get; set; }
        public CreateBpmnProcessInstanceFromBaselineCommand(int baselineId)
        {
            BaselineId = baselineId;
        }
    }
}
