using System;
using MediatR;
using Orchestra.Domain.Models;

namespace Orchestra.Application.BpmnSubProcess.Commands
{
    public class CreateSubProcessCommand : IRequest<SubProcess>
    {
        public string Name { get; set; }
        public int ProcessBaselineId { get; set; }
        public string? UserId { get; set; }
        public string? XmlContent { get; set; } // Adicionado para receber conteúdo do arquivo
    }
}