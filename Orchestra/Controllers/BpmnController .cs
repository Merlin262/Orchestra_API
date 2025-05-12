using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestra.Dtos;
using Orchestra.Handler;
using System.Xml.Linq;

namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BpmnController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BpmnController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadBpmnRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Arquivo inválido.");

            var command = new BpmnProcessCommand(request.Name, request.File);
            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Implemente o GetById se quiser retornar depois
            return Ok();
        }
    }
}
