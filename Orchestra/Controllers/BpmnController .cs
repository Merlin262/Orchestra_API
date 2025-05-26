using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Orchestra.Dtos;
using Orchestra.Handler;
using Orchestra.Handler.Command;
using Orchestra.Handler.Command.DeleteBpmnProcessBaselineCommand;
using Orchestra.Handler.Command.UploadBpmnProcessCommand;
using Orchestra.Handler.Querry;
using Orchestra.Handler.Querry.GetById;
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

            var command = new BpmnProcessCommand
            {
                //Name = request.Name,
                File = request.File
            };

            var result = await _mediator.Send(command, cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var query = new GetBpmnProcessByIdQuery(id);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllBpmnProcessesQuery();
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var command = new DeleteBpmnProcessBaselineCommand(id);
            var result = await _mediator.Send(command, cancellationToken);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
