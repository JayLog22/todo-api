using TodoApi.Core.Entities.Enums;

namespace TodoApi.DTOs;

public class GetAllTodoTaskFilterDto
{
    public bool? IsCompleted { get; set; }
    public Priority? Priority { get; set; }
}