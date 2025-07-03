﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Models;
using Microsoft.AspNetCore.SignalR;
using Orchestra.Hubs;

namespace Orchestra.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly IHubContext<TasksHub> _hubContext;
        private readonly ILogger<TasksController> _logger;

        public TasksController(
            ApplicationDbContext context,
            IMediator mediator,
            IHubContext<TasksHub> hubContext,
            ILogger<TasksController> logger)
        {
            _context = context;
            _mediator = mediator;
            _hubContext = hubContext;
            _logger = logger;
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
            var command = new Handler.Tasks.Command.AssignUser.AssignUserToTaskCommand(dto.TaskId, dto.UserId, dto.ProcessInstanceId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound("Task ou Usuário não encontrado.");

            // Log antes de enviar pelo SignalR
            _logger.LogInformation("Enviando evento SignalR UserAssignedToTask: TaskId={TaskId}, UserId={UserId}", dto.TaskId, dto.UserId);

            // Notifica todos os clientes conectados via SignalR
            await _hubContext.Clients.All.SendAsync("UserAssignedToTask", new { TaskId = dto.TaskId, UserId = dto.UserId });

            return NoContent();
        }

        [HttpGet("by-process-instance/{processInstanceId}")]
        public async Task<IActionResult> GetTasksByProcessInstance(int processInstanceId, CancellationToken cancellationToken)
        {
            var query = new Handler.BpmnInstance.Query.GetTasksForProcessInstance.GetTasksForProcessInstanceQuery(processInstanceId);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null || result.Count == 0)
                return NotFound("Nenhuma task encontrada para esse processo.");

            return Ok(result);
        }


        [HttpGet("user-process-instances/{userId}")]
        public async Task<IActionResult> GetProcessInstancesWithUserTasks(string userId, CancellationToken cancellationToken)
        {
            var query = new Handler.Tasks.Querry.GetProcessInstancesWithUserTasks.GetProcessInstancesWithUserTasksQuery(userId, cancellationToken);
            var result = await _mediator.Send(query, cancellationToken);

            if (result == null || result.Count == 0)
                return NotFound("Nenhuma task encontrada para esse processo.");

            // Log e notificação SignalR
            _logger.LogInformation("Enviando evento SignalR UserProcessInstancesFetched: UserId={UserId}, InstancesCount={Count}", userId, result.Count);
            await _hubContext.Clients.All.SendAsync("UserProcessInstancesFetched", new { UserId = userId, Instances = result });

            return Ok(result);
        }


        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateTaskStatus([FromBody] UpdateTaskStatusDto dto)
        {
            var command = new Handler.Tasks.Command.UpdateTaskStatus.UpdateTaskStatusCommand(dto.TaskId, dto.StatusId);
            var result = await _mediator.Send(command);

            if (result == null || !result.Success)
                return NotFound("Task ou Status não encontrado.");

            // Log antes de enviar pelo SignalR
            _logger.LogInformation("Enviando evento SignalR ProcessInstanceFetched: TaskId={TaskId}, StatusId={StatusId}", result.XmlTaskId, dto.StatusId);

            // Notifica todos os clientes sobre a atualização
            await _hubContext.Clients.All.SendAsync("ProcessInstanceFetched", new { TaskId = result.XmlTaskId, StatusId = dto.StatusId });

            return NoContent();
        }
    }
}
