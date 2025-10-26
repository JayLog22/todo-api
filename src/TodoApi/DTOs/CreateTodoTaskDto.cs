namespace TodoApi.DTOs;

public class CreateTodoTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? Tags { get; set; }
}