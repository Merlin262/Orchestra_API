using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace Orchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BpmnController : ControllerBase
    {
        [HttpPost("upload")]
        public async Task<IActionResult> UploadBpmnXml(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Nenhum arquivo enviado.");

            if (!file.FileName.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return BadRequest("O arquivo deve ser um .xml.");

            try
            {
                using var stream = file.OpenReadStream();
                var document = await XDocument.LoadAsync(stream, LoadOptions.None, CancellationToken.None);

                // Exemplo simples de leitura de elementos BPMN
                var processElement = document.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "process");

                if (processElement == null)
                    return BadRequest("Arquivo XML não contém um processo BPMN válido.");

                var processId = processElement.Attribute("id")?.Value;
                var processName = processElement.Attribute("name")?.Value;

                var tasks = processElement.Elements()
                    .Where(x => x.Name.LocalName.Contains("task"))
                    .Select(x => new
                    {
                        Id = x.Attribute("id")?.Value,
                        Name = x.Attribute("name")?.Value,
                        Type = x.Name.LocalName
                    })
                    .ToList();

                var result = new
                {
                    ProcessId = processId,
                    ProcessName = processName,
                    Tasks = tasks
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao processar XML: {ex.Message}");
            }
        }
    }
}
