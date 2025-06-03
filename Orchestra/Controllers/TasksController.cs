using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Models;

namespace Orchestra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tasks>>> GetTasks()
        {
            return await _context.Tasks.ToListAsync();
        }

        // GET: api/Tasks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Tasks>> GetTasks(Guid id)
        {
            var tasks = await _context.Tasks.FindAsync(id);

            if (tasks == null)
            {
                return NotFound();
            }

            return tasks;
        }

        // PUT: api/Tasks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTasks(Guid id, Tasks tasks)
        {
            if (id != tasks.Id)
            {
                return BadRequest();
            }

            _context.Entry(tasks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TasksExists(id))
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

        // POST: api/Tasks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Tasks>> PostTasks(Tasks tasks)
        {
            _context.Tasks.Add(tasks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTasks", new { id = tasks.Id }, tasks);
        }

        // DELETE: api/Tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTasks(Guid id)
        {
            var tasks = await _context.Tasks.FindAsync(id);
            if (tasks == null)
            {
                return NotFound();
            }

            _context.Tasks.Remove(tasks);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TasksExists(Guid id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        [HttpPut("assign-user")]
        public async Task<IActionResult> AssignUserToTask([FromBody] AssignUserToTaskDto dto)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.XmlTaskId == dto.TaskId);

            if (task == null)
                return NotFound("Task não encontrada.");

            var user = await _context.Users.FindAsync(dto.UserId);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            task.ResponsibleUserId = user.Id;
            task.ResponsibleUser = user;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-process-instance/{processInstanceId}")]
        public async Task<IActionResult> GetTasksByProcessInstance(int processInstanceId, CancellationToken cancellationToken)
        {
            var tasks = await _context.Tasks
                .Include(t => t.ResponsibleUser)
                .Where(t => t.BpmnProcessId == processInstanceId)
                .ToListAsync(cancellationToken);

            if (tasks == null || tasks.Count == 0)
                return NotFound("Nenhuma task encontrada para esse processo.");

            var result = tasks.Select(t => new TaskWithUserDto
            {
                TaskId = t.Id,
                XmlTaskId = t.XmlTaskId,
                Completed = t.Completed,
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


        [HttpGet("user-process-instances/{userId}")]
        public async Task<IActionResult> GetProcessInstancesWithUserTasks(string userId, CancellationToken cancellationToken)
        {
            // Busca todas as tasks do usuário
            var userTasks = await _context.Tasks
                .Include(t => t.BpmnProcess)
                .Where(t => t.ResponsibleUserId == userId)
                .ToListAsync(cancellationToken);

            // Agrupa as tasks por instância de processo
            var grouped = userTasks
                .GroupBy(t => t.BpmnProcessId)
                .ToList();

            // Busca as instâncias de processo relacionadas
            var processInstanceIds = grouped.Select(g => g.Key).ToList();
            var processInstances = await _context.bpmnProcessInstances
                .Where(pi => processInstanceIds.Contains(pi.Id))
                .ToListAsync(cancellationToken);

            // Monta o resultado
            var result = processInstances.Select(pi => new ProcessInstanceWithTasksDto
            {
                ProcessInstanceId = pi.Id,
                Name = pi.Name,
                XmlContent = pi.XmlContent,
                CreatedAt = pi.CreatedAt,
                BpmnProcessBaselineId = pi.BpmnProcessBaselineId,
                PoolNames = pi.PoolNames,
                Tasks = userTasks
                    .Where(t => t.BpmnProcessId == pi.Id)
                    .Select(t => new TaskWithUserDto
                    {
                        TaskId = t.Id,
                        XmlTaskId = t.XmlTaskId,
                        Completed = t.Completed,
                        Name = t.Name,
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
                    }).ToList()
            }).ToList();

            return Ok(result);
        }

    }
}
