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

            var newBaseline = new BpmnProcessBaseline
            {
                Name = nameToCheck,
                XmlContent = xmlContent,
                CreatedAt = DateTime.UtcNow,
                PoolNames = poolNames.Count > 0 ? poolNames : existingBaseline.PoolNames,
                CreatedBy = existingBaseline.CreatedBy,
                CreatedByUser = existingBaseline.CreatedByUser,
                Version = versionToCheck, 
                Description = request.Description ?? existingBaseline.Description
            };

            await _bpmnBaselineService.AddBaselineAsync(newBaseline, cancellationToken);
            return newBaseline;
        }
    }
}
