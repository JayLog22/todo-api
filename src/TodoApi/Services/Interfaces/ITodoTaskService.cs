using Microsoft.AspNetCore.Mvc;
using TodoApi.DTOs;

namespace TodoApi.Services.Interfaces;

public interface ITodoTaskService
{
    Task<IEnumerable<TodoTaskDto>> GetAllTasksAsync(GetAllTodoTaskFilterDto filterDto);
    Task<TodoTaskDto?> GetTaskByIdAsync(Guid id);
    Task<TodoTaskDto> CreateTaskAsync(CreateTodoTaskDto createDto);
    Task<bool> UpdateTaskAsync(Guid id, UpdateTodoTaskDto updateDto);
    Task<bool> DeleteTaskAsync(Guid id);
    Task<bool> ToggleTaskCompletionAsync(Guid id);
}