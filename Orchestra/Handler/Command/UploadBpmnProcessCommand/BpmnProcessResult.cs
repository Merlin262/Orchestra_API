using Orchestra.Models;

namespace Orchestra.Handler.Command.UploadBpmnProcessCommand
{
    public class BpmnProcessResult
    {
        public BpmnProcessBaseline Process { get; set; }
        public List<BpmnItem> Items { get; set; }
    }

}
