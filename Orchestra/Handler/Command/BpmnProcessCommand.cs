using MediatR;
using Orchestra.Models;

namespace Orchestra.Handler.Command
{
    public class BpmnProcessCommand : IRequest<BpmnProcess>
    {
        public string Name { get; set; }
        public IFormFile File { get; set; }

        public BpmnProcessCommand(string name, IFormFile file)
        {
            Name = name;
            File = file;
        }
    }
}
