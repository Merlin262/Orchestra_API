using MediatR;
using Orchestra.Models;
using Microsoft.AspNetCore.Http;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessCommand : IRequest<BpmnProcessBaseline>
    {
        public string UserId { get; set; }
        public IFormFile File { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
