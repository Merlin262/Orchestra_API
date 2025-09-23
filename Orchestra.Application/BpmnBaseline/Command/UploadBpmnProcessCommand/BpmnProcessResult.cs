using Orchestra.Models;

namespace Orchestra.Handler.BpmnBaseline.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessResult
    {
        public BpmnProcessBaseline Process { get; set; }
        public List<BpmnItem> Items { get; set; }
    }

}
