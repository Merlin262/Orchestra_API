using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Enums;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProcessVersionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ProcessVersionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Endpoint principal para trazer o resumo do processo (mock do print)
        [HttpGet("summary/{baselineId:int}")]
        public async Task<IActionResult> GetProcessSummary(int baselineId)
        {
            // Busca o baseline pelo id
            var baseline = await _context.BpmnProcess.FirstOrDefaultAsync(b => b.Id == baselineId);
            if (baseline == null)
                return NotFound("Baseline não encontrada.");

            var processName = baseline.Name;

            // Baselines
            var baselines = await _context.BpmnProcess
                .Where(b => b.Name == processName)
                .OrderBy(b => b.Version)
                .ToListAsync();

            // BaselineHistory
            var baselineHistories = await _context.BaselineHistories
                .Where(h => h.BpmnProcessBaselineId == baselineId)
                .OrderBy(h => h.Version)
                .ToListAsync();

            // Instâncias - busca por baselineId
            var instances = await _context.bpmnProcessInstances
                .Where(i => i.BpmnProcessBaselineId == baselineId)
                .ToListAsync();

            // Total de instâncias
            var totalInstances = instances.Count;
            // Versão ativa (maior versão)
            var activeBaseline = baselines.OrderByDescending(b => b.Version).FirstOrDefault();
            // Baselines count
            var baselinesCount = baselines.Count;
            // Instâncias ativas (exemplo: Status != Finished)
            var activeInstances = instances.Where(i => i.Status != StatusEnum.Finished).ToList();

            return Ok(new
            {
                BaselinesCount = baselinesCount,
                InstancesCount = activeInstances.Count,
                ActiveVersion = activeBaseline?.Version,
                TotalInstances = totalInstances,
                Baselines = baselineHistories.Select(h => new {
                    Version = h.Version,
                    Status = GetBaselineStatus(h, activeBaseline),
                    Name = h.Name,
                    Description = h.Description,
                    ChangedBy = h.ChangedBy,
                    ChangedAt = h.ChangedAt,
                    ChangeType = h.ChangeType,
                    Responsible = h.Responsible != null ? _context.Users.FirstOrDefault(u => u.Id == h.Responsible)?.FullName : null,
                    InstancesCount = instances.Count(i => i.version == h.Version)
                }).OrderBy(h => h.Version).ToList(),
                ActiveInstances = activeInstances.Select(i => new {
                    Version = i.version,
                    Name = i.Name,
                    Responsible = i.BpmnProcessBaseline.CreatedByUser != null ? i.BpmnProcessBaseline.CreatedByUser.FullName : null,
                    CreatedAt = i.CreatedAt,
                    Status = i.Status.ToString(),
                    Progress = GetInstanceProgress(i)
                }).ToList()
            });
        }

        private string GetBaselineStatus(BaselineHistory h, BpmnProcessBaseline? activeBaseline)
        {
            if (activeBaseline != null && h.Version == activeBaseline.Version)
                return "Ativa";
            if (h.ChangeType == "Upload")
                return "Arquivada";
            return "Rascunho";
        }

        private object GetInstanceProgress(BpmnProcessInstance instance)
        {
            // Exemplo: calcular progresso baseado em tasks
            var totalTasks = _context.Tasks.Count(t => t.BpmnProcessId == instance.Id);
            var completedTasks = _context.Tasks.Count(t => t.BpmnProcessId == instance.Id && t.Status == StatusEnum.Finished);
            return new { Completed = completedTasks, Total = totalTasks };
        }
    }
}
