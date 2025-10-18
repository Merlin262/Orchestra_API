using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MediatR;
using Orchestra.Data.Context;
using Orchestra.Domain.Models;
using Orchestra.Application.BpmnSubProcess.Commands;
using Orchestra.DTOs;

namespace Orchestra.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubProcessesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public SubProcessesController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        // GET: api/SubProcesses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubProcess>>> GetSubProcesses()
        {
            return await _context.SubProcesses.ToListAsync();
        }

        // GET: api/SubProcesses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SubProcess>> GetSubProcess(Guid id)
        {
            var subProcess = await _context.SubProcesses.FindAsync(id);

            if (subProcess == null)
            {
                return NotFound();
            }

            return subProcess;
        }

        // PUT: api/SubProcesses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSubProcess(Guid id, SubProcess subProcess)
        {
            if (id != subProcess.Id)
            {
                return BadRequest();
            }

            _context.Entry(subProcess).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubProcessExists(id))
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

        // POST: api/SubProcesses/upload
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<IEnumerable<SubProcess>>> PostSubProcessesWithFiles(
        [FromForm] int processBaselineId,
        [FromForm] List<CreateSubProcessWithFileDto> subProcesses)
        {
            var result = new List<SubProcess>();
            foreach (var dto in subProcesses)
            {
                var command = new CreateSubProcessCommand
                {
                    Name = dto.Name,
                    ProcessBaselineId = processBaselineId, // Usa o parâmetro recebido
                    UserId = dto.UserId,
                };
                if (dto.File != null)
                {
                    using var stream = new MemoryStream();
                    await dto.File.CopyToAsync(stream);
                    command.XmlContent = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                }
                var subProcess = await _mediator.Send(command);
                result.Add(subProcess);
            }
            return Ok(result);
        }

        // DELETE: api/SubProcesses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubProcess(Guid id)
        {
            var subProcess = await _context.SubProcesses.FindAsync(id);
            if (subProcess == null)
            {
                return NotFound();
            }

            _context.SubProcesses.Remove(subProcess);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/SubProcesses/by-instance/{instanceId}
        [HttpGet("by-instance/{instanceId}")]
        public async Task<ActionResult<IEnumerable<SubProcessWithTasksDto>>> GetSubProcessesByInstanceId(int instanceId)
        {
            // Busca a instância para obter a versão
            var instance = await _context.bpmnProcessInstances.FindAsync(instanceId);
            if (instance == null)
            {
                return NotFound($"Instância com id {instanceId} não encontrada.");
            }

            var subProcesses = await _context.SubProcesses
                .Where(sp => sp.Tasks.Any(t => t.BpmnProcessId == instanceId) && sp.BaselineVersion == instance.version)
                .Include(sp => sp.Tasks.Where(t => t.BpmnProcessId == instanceId))
                .Select(sp => new SubProcessWithTasksDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    XmlContent = sp.XmlContent,
                    ProcessBaselineId = sp.ProcessBaselineId,
                    UserId = sp.UserId,
                    BaselineHistoryId = sp.BaselineHistoryId,
                    Tasks = sp.Tasks.Where(t => t.BpmnProcessId == instanceId).Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        XmlTaskId = t.XmlTaskId,
                        Completed = t.Completed,
                        StatusId = (int)t.Status,
                        Comments = t.Comments,
                        ResponsibleUserId = t.ResponsibleUserId,
                        Pool = t.Pool
                    }).ToList()
                })
                .ToListAsync();
            return Ok(subProcesses);
        }

        // GET: api/SubProcesses/by-baseline/{baselineId}
        [HttpGet("by-baseline/{baselineId}")]
        public async Task<ActionResult<IEnumerable<SubProcessWithTasksDto>>> GetSubProcessesByBaselineId(int baselineId)
        {
            // Busca a baseline para obter a versão
            var baseline = await _context.BpmnProcess.FindAsync(baselineId);
            if (baseline == null)
            {
                return NotFound($"Baseline com id {baselineId} não encontrada.");
            }

            // Busca o BaselineHistory mais recente para esta baseline
            var latestBaselineHistory = await _context.BaselineHistories
                .Where(bh => bh.BpmnProcessBaselineId == baselineId)
                .OrderByDescending(bh => bh.Version)
                .ThenByDescending(bh => bh.ChangedAt)
                .FirstOrDefaultAsync();

            if (latestBaselineHistory == null)
            {
                return NotFound($"Nenhum histórico encontrado para a baseline {baselineId}.");
            }

            var subProcesses = await _context.SubProcesses
                .Where(sp => sp.ProcessBaselineId == baselineId && sp.BaselineHistoryId == latestBaselineHistory.Id)
                .Include(sp => sp.Tasks)
                .Select(sp => new SubProcessWithTasksDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    XmlContent = sp.XmlContent,
                    ProcessBaselineId = sp.ProcessBaselineId,
                    UserId = sp.UserId,
                    Version = sp.BaselineVersion,
                    BaselineHistoryId = sp.BaselineHistoryId,
                    Tasks = sp.Tasks.Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Description = t.Description,
                        XmlTaskId = t.XmlTaskId,
                        Completed = t.Completed,
                        StatusId = (int)t.Status,
                        Comments = t.Comments,
                        ResponsibleUserId = t.ResponsibleUserId,
                        Pool = t.Pool
                    }).ToList()
                })
                .ToListAsync();
            return Ok(subProcesses);
        }

        private bool SubProcessExists(Guid id)
        {
            return _context.SubProcesses.Any(e => e.Id == id);
        }
    }
}
