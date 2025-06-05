using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Orchestra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BpmnProcessInstancesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BpmnProcessInstancesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/BpmnProcessInstances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BpmnProcessInstance>>> GetbpmnProcessInstances()
        {
            return await _context.bpmnProcessInstances.ToListAsync();
        }

        // GET: api/BpmnProcessInstances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BpmnProcessInstance>> GetBpmnProcessInstance(int id)
        {
            var bpmnProcessInstance = await _context.bpmnProcessInstances.FindAsync(id);

            if (bpmnProcessInstance == null)
            {
                return NotFound();
            }

            return bpmnProcessInstance;
        }

        // PUT: api/BpmnProcessInstances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBpmnProcessInstance(int id, BpmnProcessInstance bpmnProcessInstance)
        {
            if (id != bpmnProcessInstance.Id)
            {
                return BadRequest();
            }

            _context.Entry(bpmnProcessInstance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BpmnProcessInstanceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BpmnProcessInstances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BpmnProcessInstance>> PostBpmnProcessInstance(BpmnProcessInstance bpmnProcessInstance)
        {
            _context.bpmnProcessInstances.Add(bpmnProcessInstance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBpmnProcessInstance", new { id = bpmnProcessInstance.Id }, bpmnProcessInstance);
        }

        // DELETE: api/BpmnProcessInstances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBpmnProcessInstance(int id)
        {
            var bpmnProcessInstance = await _context.bpmnProcessInstances.FindAsync(id);
            if (bpmnProcessInstance == null)
            {
                return NotFound();
            }

            _context.bpmnProcessInstances.Remove(bpmnProcessInstance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BpmnProcessInstanceExists(int id)
        {
            return _context.bpmnProcessInstances.Any(e => e.Id == id);
        }

        // POST: api/BpmnProcessInstances/CreateFromBaseline/5
        [HttpPost("CreateFromBaseline/{baselineId}")]
        public async Task<ActionResult<BpmnProcessInstance>> CreateFromBaseline(int baselineId)
        {
            var baseline = await _context.BpmnProcess.FindAsync(baselineId);
            if (baseline == null)
            {
                return NotFound($"BpmnProcessBaseline com id {baselineId} não encontrado.");
            }

            var instance = new BpmnProcessInstance
            {
                Name = baseline.Name,
                XmlContent = baseline.XmlContent,
                BpmnProcessBaselineId = baseline.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.bpmnProcessInstances.Add(instance);
            await _context.SaveChangesAsync();

            // Parse o XML para criar ProcessSteps e mapear pelo bpmnId
            var xDoc = XDocument.Parse(baseline.XmlContent ?? "");
            XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";
            var processSteps = new List<ProcessStep>();
            var stepMap = new Dictionary<string, ProcessStep>();
            var elementTypes = new[] { "startEvent", "task", "userTask", "scriptTask", "exclusiveGateway", "endEvent" };

            foreach (var type in elementTypes)
            {
                var elements = xDoc.Descendants(bpmn + type);
                foreach (var element in elements)
                {
                    var bpmnId = element.Attribute("id")?.Value ?? Guid.NewGuid().ToString();
                    var step = new ProcessStep
                    {
                        Id = Guid.NewGuid(),
                        BpmnId = bpmnId,
                        Name = element.Attribute("name")?.Value ?? type,
                        Type = type,
                        BpmnProcessId = instance.Id
                    };
                    processSteps.Add(step);
                    stepMap[bpmnId] = step;
                }
            }

            if (processSteps.Any())
            {
                _context.ProcessStep.AddRange(processSteps);
                await _context.SaveChangesAsync();
            }

            // Agora crie as Tasks associando ao ProcessStep correto
            var tasks = new List<Tasks>();
            var taskElements = xDoc.Descendants(bpmn + "task");
            foreach (var element in taskElements)
            {
                var bpmnId = element.Attribute("id")?.Value;
                var taskName = element.Attribute("name")?.Value; // <-- Captura o nome do XML
                if (!string.IsNullOrEmpty(bpmnId) && stepMap.TryGetValue(bpmnId, out var step))
                {
                    var task = new Tasks
                    {
                        Id = Guid.NewGuid(),
                        Name = taskName,
                        XmlTaskId = bpmnId,
                        BpmnProcessId = instance.Id,
                        BpmnProcess = instance,
                        ProcessStepId = step.Id,
                        ProcessStep = step,
                        ResponsibleUserId = null,
                        ResponsibleUser = null,
                        Completed = false,
                        CreatedAt = DateTime.UtcNow,
                        CompletedAt = null,
                        Comments = null,
                        StatusId = 3
                    };
                    tasks.Add(task);
                }
            }

            if (tasks.Any())
            {
                _context.Tasks.AddRange(tasks);
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetBpmnProcessInstance), new { id = instance.Id }, instance);
        }


        // GET: api/BpmnProcessInstances/{id}/tasks
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<TaskWithUserDto>>> GetTasksForProcessInstance(int id)
        {
            var processInstance = await _context.bpmnProcessInstances.FindAsync(id);
            if (processInstance == null)
                return NotFound();

            var tasks = await _context.Tasks
                .Where(t => t.BpmnProcessId == id)
                .Include(t => t.ResponsibleUser)
                .Include(t => t.Status)
                .ToListAsync();

            var result = tasks.Select(t => new TaskWithUserDto
            {
                TaskId = t.Id,
                Name = t.Name,
                XmlTaskId = t.XmlTaskId,
                Completed = t.Completed,
                StatusId = t.StatusId,
                CreatedAt = t.CreatedAt,
                CompletedAt = t.CompletedAt,
                Comments = t.Comments,
                ResponsibleUser = t.ResponsibleUser == null ? null : new UserDto
                {
                    Id = t.ResponsibleUser.Id,
                    UserName = t.ResponsibleUser.UserName,
                    Email = t.ResponsibleUser.Email,
                    FullName = t.ResponsibleUser.FullName,
                    Role = t.ResponsibleUser.Role
                }
            }).ToList();

            return Ok(result);
        }
    }
}
