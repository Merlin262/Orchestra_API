using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Handler.BpmnInstance.Command;
using Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceCommand;
using Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceFromBaselineCommand;
using Orchestra.Handler.BpmnInstance.Command.DeleteInstance;
using Orchestra.Handler.BpmnInstance.Query.GetById;
using Orchestra.Handler.BpmnInstance.Query.GetProcessInstance;
using Orchestra.Handler.BpmnInstance.Query.GetTasksForProcessInstance;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Orchestra.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BpmnProcessInstancesController : ControllerBase
    {
        //private readonly ApplicationDbContext _context;
        //private readonly IBpmnProcessInstanceService _bpmnProcessInstanceService;
        private readonly IMediator _mediator;

        public BpmnProcessInstancesController(ApplicationDbContext context, IMediator mediator)
        {
            //_context = context;
            //_bpmnProcessInstanceService = bpmnProcessInstanceService;
            _mediator = mediator;
        }

        // GET: api/BpmnProcessInstances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BpmnProcessInstance>>> GetbpmnProcessInstances()
        {
            var result = await _mediator.Send(new GetBpmnProcessInstancesQuery());
            return Ok(result);
        }

        // GET: api/BpmnProcessInstances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BpmnProcessInstance>> GetBpmnProcessInstance(int id)
        {
            var bpmnProcessInstance = await _mediator.Send(new GetBpmnProcessInstanceByIdQuery(id));

            if (bpmnProcessInstance == null)
            {
                return NotFound();
            }

            return bpmnProcessInstance;
        }


        // PUT: api/BpmnProcessInstances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBpmnProcessInstance(int id, BpmnProcessInstance bpmnProcessInstance)
        //{
        //    if (id != bpmnProcessInstance.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(bpmnProcessInstance).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BpmnProcessInstanceExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/BpmnProcessInstances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BpmnProcessInstance>> PostBpmnProcessInstance(BpmnProcessInstance bpmnProcessInstance)
        {
            var created = await _mediator.Send(new CreateBpmnProcessInstanceCommand(bpmnProcessInstance));
            return CreatedAtAction(nameof(GetBpmnProcessInstance), new { id = created.Id }, created);
        }


        // DELETE: api/BpmnProcessInstances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBpmnProcessInstance(int id)
        {
            var result = await _mediator.Send(new DeleteBpmnProcessInstanceCommand(id));
            if (!result)
                return NotFound();

            return NoContent();
        }


        // POST: api/BpmnProcessInstances/CreateFromBaseline/5
        [HttpPost("CreateFromBaseline/{baselineId}")]
        public async Task<ActionResult<BpmnProcessInstance>> CreateFromBaseline(int baselineId)
        {
            try
            {
                var command = new CreateBpmnProcessInstanceFromBaselineCommand(baselineId);
                var instance = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetBpmnProcessInstance), new { id = instance.Id }, instance);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/BpmnProcessInstances/{id}/tasks
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskWithUserDto>>> GetTasksForProcessInstance(int id)
        {
            var result = await _mediator.Send(new GetTasksForProcessInstanceQuery(id));
            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }
    }
}
