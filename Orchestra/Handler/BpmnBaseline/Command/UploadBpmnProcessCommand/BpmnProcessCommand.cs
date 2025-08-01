using MediatR;
using Orchestra.Models;
using MediatR;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessCommand : IRequest<BpmnProcessBaseline>
    {
        public string UserId { get; set; }
        public IFormFile File { get; set; }
    }

}
