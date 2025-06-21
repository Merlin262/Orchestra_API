namespace Orchestra.Dtos
{
    public class UpdateTaskStatusDto
    {
        public Guid TaskId { get; set; }
        public int StatusId { get; set; }
    }
}
