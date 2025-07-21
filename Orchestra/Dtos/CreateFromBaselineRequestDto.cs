// DTO para receber os dados do request
public class CreateFromBaselineRequestDto
{
    public int BaselineId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
