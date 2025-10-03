using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Orchestra.Data.Context;
using Orchestra.Dtos;
using Orchestra.Handler.BpmnInstance.Command;
using Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceCommand;
using Orchestra.Handler.BpmnInstance.Command.CreateBpmnProcessInstanceFromBaselineCommand;
using Orchestra.Handler.BpmnInstance.Command.DeleteInstance;
using Orchestra.Handler.BpmnInstance.Command.UpdateInstance;
using Orchestra.Handler.BpmnInstance.Query;
using Orchestra.Handler.BpmnInstance.Query.GetById;
using Orchestra.Handler.BpmnInstance.Query.GetProcessInstance;
using Orchestra.Handler.BpmnInstance.Query.GetTasksForProcessInstance;
using Orchestra.Handler.Tasks.Querry.GetTaskWithUser;
using Orchestra.Hubs;
using Orchestra.Infrastructure.Repositories;
using Orchestra.Models;
using Orchestra.Models.Orchestra.Models;
using Orchestra.Repoitories.Interfaces;
using Orchestra.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Orchestra.Enums;

namespace Orchestra.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BpmnInstancesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<TasksHub> _hubContext;
        private readonly ILogger<BpmnInstancesController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;

        public BpmnInstancesController(
            ApplicationDbContext context,
            IMediator mediator,
            IHubContext<TasksHub> hubContext,
            ILogger<BpmnInstancesController> logger, 
            IUserRepository userRepository)
        {
            _mediator = mediator;
            _hubContext = hubContext;
            _logger = logger;
            _context = context;
            _userRepository = userRepository;
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

            // Log antes de enviar pelo SignalR
            _logger.LogInformation("Enviando evento SignalR ProcessInstanceFetched: ProcessInstanceId={ProcessInstanceId}", bpmnProcessInstance.Id);

            // Envia mensagem para todos os clientes conectados ao hub
            await _hubContext.Clients.All.SendAsync("ProcessInstanceFetched", bpmnProcessInstance);

            return bpmnProcessInstance;
        }

        // POST: api/BpmnProcessInstances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BpmnProcessInstance>> PostBpmnProcessInstance(BpmnProcessInstance bpmnProcessInstance)
        {
            var created = await _mediator.Send(new CreateBpmnProcessInstanceCommand(bpmnProcessInstance));
            return CreatedAtAction(nameof(GetBpmnProcessInstance), new { id = created.Id }, created);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBpmnProcessInstance(int id)
        {
            var result = await _mediator.Send(new DeleteBpmnProcessInstanceCommand(id));
            if (!result)
                return NotFound();

            return NoContent();
        }


        // POST: api/BpmnProcessInstances/CreateFromBaseline
        [HttpPost("CreateFromBaseline")]
        public async Task<ActionResult<BpmnProcessInstance>> CreateFromBaseline([FromBody] CreateFromBaselineRequestDto dto)
        {
            try
            {
                var command = new CreateBpmnProcessInstanceFromBaselineCommand(dto.BaselineId, dto.Name, dto.Description, dto.UserId);
                var instance = await _mediator.Send(command);
                //instance.CreatedBy = user;
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

        // GET: api/BpmnProcessInstances/{id}/tasks-with-users-status
        [HttpGet("{id}/tasks-with-users-status")]
        public async Task<ActionResult<IEnumerable<TaskWithUserDto>>> GetTasksWithUsersAndStatus(int id)
        {
            var result = await _mediator.Send(new GetTaskWithUserQuery(id));
            if (result == null || !result.Any())
                return NotFound();

            return Ok(result);
        }

        // PUT: api/BpmnProcessInstances/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBpmnProcessInstance(int id, [FromBody] UpdateBpmnProcessInstanceDto dto)
        {
            var command = new UpdateBpmnProcessInstanceCommand(id, dto.Name, dto.Description);
            var updated = await _mediator.Send(command);
            if (updated == null)
                return NotFound();
            return Ok(updated);
        }

        // GET: api/BpmnProcessInstances/by-responsible/{userId}
        [HttpGet("by-responsible/{userId}")]
        public async Task<ActionResult<IEnumerable<ProcessInstanceWithTasksDto>>> GetProcessInstancesByResponsibleUser(string userId)
        {
            var result = await _mediator.Send(new GetProcessInstancesByResponsibleUserQuery(userId));
            if (result == null || !result.Any())
                return NotFound();
            return Ok(result);
        }

        // GET: api/BpmnProcessInstances/{id}/is-previous-task-completed-before-gateway
        [HttpGet("{id}/is-previous-task-completed-before-gateway")]
        public async Task<ActionResult<object>> IsPreviousTaskCompletedBeforeGateway(int id)
        {
            var baseline = await _context.bpmnProcessInstances
                .AsNoTracking()
                .Where(s => s.Id == id)
                .ToListAsync();

            var baselineInstance = baseline.FirstOrDefault();
            if (baselineInstance == null)
                return NotFound("Instância do processo não encontrada.");

            var steps = await _context.ProcessStep
                .AsNoTracking()
                .Where(s => s.BpmnProcessId == baselineInstance.BpmnProcessBaselineId)
                .ToListAsync();

            var tasks = await _context.Tasks
                .AsNoTracking()
                .Where(t => t.BpmnProcessId == id)
                .ToListAsync();

            var xmlContent = baselineInstance.XmlContent;
            var xdoc = string.IsNullOrEmpty(xmlContent) ? null : System.Xml.Linq.XDocument.Parse(xmlContent);

            var gateways = steps.Where(s => s.Type == "exclusiveGateway").ToList();
            bool isCompleted = false;
            var gatewayResults = new List<object>();

            foreach (var gateway in gateways)
            {
                var previousSteps = steps.Where(s => s.NextStepId == gateway.BpmnId && (s.Type == "task" || s.Type == "userTask")).ToList();
                var previousTasks = tasks.Where(t => previousSteps.Any(ps => ps.BpmnId == t.XmlTaskId) && t.Status == StatusEnum.Finished).ToList();
                if (!previousTasks.Any())
                    continue;

                isCompleted = true;

                var nextSteps = steps.Where(s => s.LastStepId == gateway.BpmnId && (s.Type == "task" || s.Type == "userTask")).ToList();
                var options = new List<object>();
                var allPathIds = new List<HashSet<string>>();

                // Primeiro, para cada caminho, percorre e guarda os BpmnIds visitados
                foreach (var startStep in nextSteps)
                {
                    var pathIds = new HashSet<string>();
                    var currentStep = startStep;
                    while (currentStep != null && (currentStep.Type == "task" || currentStep.Type == "userTask"))
                    {
                        if (pathIds.Contains(currentStep.BpmnId)) break;
                        pathIds.Add(currentStep.BpmnId);
                        if (string.IsNullOrEmpty(currentStep.NextStepId)) break;
                        var nextStep = steps.FirstOrDefault(s => s.BpmnId == currentStep.NextStepId);
                        if (nextStep == null || nextStep.Type == "exclusiveGateway") break;
                        currentStep = nextStep;
                    }
                    allPathIds.Add(pathIds);
                }

                // Agora, para cada caminho, só inclui tasks exclusivas
                for (int i = 0; i < nextSteps.Count; i++)
                {
                    var startStep = nextSteps[i];
                    var pathIds = allPathIds[i];
                    // Percorre o caminho até encontrar um gateway
                    var currentStep = startStep;
                    bool foundGateway = false;
                    while (currentStep != null)
                    {
                        if (currentStep != startStep && currentStep.Type == "exclusiveGateway")
                        {
                            foundGateway = true;
                            break;
                        }
                        if (string.IsNullOrEmpty(currentStep.NextStepId)) break;
                        currentStep = steps.FirstOrDefault(s => s.BpmnId == currentStep.NextStepId);
                    }
                    var pathTasks = new List<object>();
                    if (foundGateway)
                    {
                        // Se encontrou um gateway, retorna apenas a primeira task do caminho
                        var firstTask = tasks.Where(t => t.XmlTaskId == startStep.BpmnId).Select(t => new {
                            t.Id,
                            t.XmlTaskId,
                            t.Status,
                            t.Name,
                            t.Completed,
                            t.CompletedAt,
                            t.ResponsibleUserId
                        }).FirstOrDefault();
                        if (firstTask != null)
                        {
                            pathTasks.Add(firstTask);
                        }
                    }
                    else
                    {
                        // Caso contrário, segue a lógica anterior de exclusividade
                        var exclusiveIds = pathIds.Where(id => !allPathIds.Where((set, idx) => idx != i).Any(set => set.Contains(id))).ToList();
                        foreach (var bpmnId in exclusiveIds)
                        {
                            var stepTasks = tasks.Where(t => t.XmlTaskId == bpmnId).Select(t => new {
                                t.Id,
                                t.XmlTaskId,
                                t.Status,
                                t.Name,
                                t.Completed,
                                t.CompletedAt,
                                t.ResponsibleUserId
                            }).ToList();
                            pathTasks.AddRange(stepTasks);
                        }
                        // Se não houver tasks exclusivas, inclui a primeira task do caminho
                        if (!pathTasks.Any())
                        {
                            var firstTask = tasks.Where(t => t.XmlTaskId == startStep.BpmnId).Select(t => new {
                                t.Id,
                                t.XmlTaskId,
                                t.Status,
                                t.Name,
                                t.Completed,
                                t.CompletedAt,
                                t.ResponsibleUserId
                            }).FirstOrDefault();
                            if (firstTask != null)
                            {
                                pathTasks.Add(firstTask);
                            }
                        }
                    }
                    // Busca o texto da opção no XML
                    string optionText = startStep.Name;
                    if (xdoc != null)
                    {
                        var seqFlow = xdoc.Descendants()
                            .FirstOrDefault(x => x.Name.LocalName == "sequenceFlow" &&
                                (string)x.Attribute("sourceRef") == gateway.BpmnId &&
                                (string)x.Attribute("targetRef") == startStep.BpmnId);
                        if (seqFlow != null && seqFlow.Attribute("name") != null)
                        {
                            optionText = seqFlow.Attribute("name").Value;
                        }
                    }

                    // Identifica a última task do caminho e verifica se status == 3
                    var loopTasks = new List<object>();
                    if (pathTasks.Count > 0)
                    {
                        var lastTask = pathTasks.Last();
                        // status == 3 (loop)
                        var statusProp = lastTask.GetType().GetProperty("Status");
                        if (statusProp != null && (int)statusProp.GetValue(lastTask) == 3)
                        {
                            loopTasks.Add(lastTask);
                            // Remove a última task de pathTasks para evitar duplicidade
                            pathTasks.RemoveAt(pathTasks.Count - 1);
                        }
                    }

                    options.Add(new {
                        pathId = startStep.BpmnId,
                        optionText,
                        tasks = pathTasks,
                        loopTasks = loopTasks.Count > 0 ? loopTasks : null
                    });
                }
                gatewayResults.Add(new {
                    gatewayId = gateway.BpmnId,
                    gatewayName = gateway.Name,
                    options = options
                });
            }

            return Ok(new {
                isCompleted,
                gateways = gatewayResults
            });
        }

    }
}
