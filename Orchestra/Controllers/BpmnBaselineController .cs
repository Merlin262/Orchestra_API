using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Handler;
using Orchestra.Handler.BpmnBaseline.Command.DeleteBpmnProcessBaselineCommand;
using Orchestra.Handler.BpmnBaseline.Command.UpdateBpmnProcessBaselineCommand;
using Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand;
using Orchestra.Handler.BpmnBaseline.Querry.GetAll;
using Orchestra.Handler.BpmnBaseline.Querry.GetById;
using Orchestra.Models;
using System.Xml.Linq;

namespace Orchestra.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BpmnController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;


        public BpmnController(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadBpmnRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Arquivo inválido.");

            var command = new BpmnProcessCommand
            {
                UserId = request.UserId,
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

        [HttpGet("{id}/pools")]
        public async Task<IActionResult> GetPoolNames(int id, CancellationToken cancellationToken)
        {
            var process = await _context.BpmnProcess
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

            if (process == null)
                return NotFound();

            return Ok(process.PoolNames);
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetByCreatedByUserId(string userId, CancellationToken cancellationToken)
        {
            var query = new Handler.BpmnBaseline.Querry.GetByUser.GetProcessBaselineByUser(userId);
            var baselines = await _mediator.Send(query, cancellationToken);

            if (baselines == null || baselines.Count == 0)
                return NotFound();

            return Ok(baselines);
        }

        [HttpPut("{id}/update-baseline")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateBaselineWithNewBpmn(int id, [FromForm] UpdateBaselineWithNewBpmnRequest request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Arquivo inválido.");

            var command = new UpdateBpmnProcessBaselineCommand
            {
                Id = id,
                File = request.File,
                Name = request.Name,
                Description = request.Description,
                //Version = request.Version
            };

            try
            {
                var result = await _mediator.Send(command, cancellationToken);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
