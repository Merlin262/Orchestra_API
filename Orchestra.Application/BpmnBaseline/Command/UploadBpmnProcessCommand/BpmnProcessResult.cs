using Orchestra.Models;
using System.Collections.Generic;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessResult
    {
        public BpmnProcessBaseline Process { get; set; }
        public List<BpmnItem> Items { get; set; }
        public bool HasSubProcess { get; set; } // Indica se o processo possui subprocessos
        public List<string> SubProcessNames { get; set; } // Nomes dos subprocessos
    }
}
