using Orchestra.Models;
using System.Collections.Generic;

namespace Orchestra.Handler.BpmnBaseline.Command.UpdateBpmnProcessBaselineCommand
{
    public class UpdateBpmnProcessBaselineCommandResult
    {
        public BpmnProcessBaseline Process { get; set; }
        public bool HasSubProcess { get; set; }
        public List<string> SubProcessNames { get; set; } 
    }
}
