using MediatR;
using Orchestra.Models;
using Orchestra.Serviecs.Intefaces;
using System.Xml.Linq;

namespace Orchestra.Handler.BpmnBaseline.Command.UpdateBpmnProcessBaselineCommand
{
    public class UpdateBpmnProcessBaselineCommandHandler : IRequestHandler<UpdateBpmnProcessBaselineCommand, BpmnProcessBaseline>
    {
        private readonly IBpmnBaselineService _bpmnBaselineService;

        public UpdateBpmnProcessBaselineCommandHandler(IBpmnBaselineService bpmnBaselineService)
        {
            _bpmnBaselineService = bpmnBaselineService;
        }

        public async Task<BpmnProcessBaseline> Handle(UpdateBpmnProcessBaselineCommand request, CancellationToken cancellationToken)
        {
            if (request.File == null || request.File.Length == 0)
                return null!;

            var existingBaseline = await _bpmnBaselineService.GetByIdAsync(request.Id, cancellationToken);
            if (existingBaseline == null)
                return null!;

            string xmlContent;
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            {
                xmlContent = await reader.ReadToEndAsync();
            }

            xmlContent = _bpmnBaselineService.FixDataObjectToDataObjectReference(xmlContent);

            string? processName = null;
            try
            {
                var xDoc = XDocument.Parse(xmlContent);
                XNamespace bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";
                var processElement = xDoc.Descendants(bpmn + "process").LastOrDefault();
                processName = processElement?.Attribute("name")?.Value;
            }
            catch { }

            var nameToCheck = request.Name ?? processName ?? existingBaseline.Name;
            // Sempre adicione 0.1 à versão já existente do processo
            var versionToCheck = Math.Round(existingBaseline.Version + 0.1, 1, MidpointRounding.AwayFromZero);

            var poolNames = _bpmnBaselineService.ExtractPoolNames(xmlContent);

            // Atualiza o baseline existente em vez de criar um novo
            existingBaseline.Name = nameToCheck;
            existingBaseline.XmlContent = xmlContent;
            existingBaseline.CreatedAt = DateTime.UtcNow;
            existingBaseline.PoolNames = poolNames.Count > 0 ? poolNames : existingBaseline.PoolNames;
            existingBaseline.Version = versionToCheck;
            existingBaseline.Description = request.Description ?? existingBaseline.Description;
            existingBaseline.IsActive = true;

            await _bpmnBaselineService.UpdateBaselineAsync(existingBaseline, cancellationToken);

            // Save BaselineHistory (Edit)
            var dbContext = _bpmnBaselineService as Orchestra.Serviecs.BpmnBaselineService;
            var contextField = dbContext?.GetType().GetField("_bpmnProcessBaselineRepository", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var repo = contextField?.GetValue(dbContext) as Orchestra.Repoitories.BpmnProcessBaselineRepository;
            var context = repo?.GetType().GetField("_context", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(repo) as Orchestra.Data.Context.ApplicationDbContext;
            if (context != null)
            {
                var history = new BaselineHistory
                {
                    BpmnProcessBaselineId = existingBaseline.Id,
                    Name = existingBaseline.Name,
                    XmlContent = existingBaseline.XmlContent,
                    Description = existingBaseline.Description,
                    Version = existingBaseline.Version,
                    ChangedBy = existingBaseline.CreatedByUserId,
                    ChangedAt = existingBaseline.CreatedAt,
                    ChangeType = "Edit",
                    Responsible = existingBaseline.CreatedByUserId
                };
                context.BaselineHistories.Add(history);
                await context.SaveChangesAsync(cancellationToken);
            }
            return existingBaseline;
        }
    }
}
