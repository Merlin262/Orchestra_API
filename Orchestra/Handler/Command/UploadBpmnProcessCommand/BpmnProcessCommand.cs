using MediatR;
using Orchestra.Models;
using MediatR;

namespace Orchestra.Handler.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessCommand : IRequest<BpmnProcessBaseline>
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }
    }

}
