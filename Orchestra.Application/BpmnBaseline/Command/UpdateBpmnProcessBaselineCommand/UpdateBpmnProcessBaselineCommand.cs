using MediatR;
using Microsoft.AspNetCore.Http;
using Orchestra.Models;

namespace Orchestra.Handler.BpmnBaseline.Command.UpdateBpmnProcessBaselineCommand
{
    public class UpdateBpmnProcessBaselineCommand : IRequest<BpmnProcessBaseline>
    {
        public int Id { get; set; }
        public IFormFile File { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
