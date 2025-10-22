namespace TodoApi.DTOs;

public class TodoTaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? Tags { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateTodoTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? Tags { get; set; }
}

public class UpdateTodoTaskDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public string Priority { get; set; } = "Medium";
    public string? Tags { get; set; }
}