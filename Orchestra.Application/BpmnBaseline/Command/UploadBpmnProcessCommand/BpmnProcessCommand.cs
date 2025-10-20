using MediatR;
using Orchestra.Models;
using Microsoft.AspNetCore.Http;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    // Altere o retorno para BpmnProcessResult
    public class BpmnProcessCommand : IRequest<BpmnProcessResult>
    {
        public string UserId { get; set; }
        public IFormFile File { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
