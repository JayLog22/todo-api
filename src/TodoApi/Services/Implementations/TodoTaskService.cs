using AutoMapper;
using TodoApi.Controllers;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;
using TodoApi.DTOs;
using TodoApi.Services.Interfaces;
using TodoApi.TodoApi.Core.Interfaces;

namespace TodoApi.Services.Implementations;

public class TodoTaskService : ITodoTaskService
{
    private readonly ITodoTaskRepository _repository;
    private readonly ILogger<TodoTaskService> _logger;
    private readonly IMapper _mapper;

    public TodoTaskService(ITodoTaskRepository repository, ILogger<TodoTaskService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TodoTaskDto>> GetAllTasksAsync(bool? isCompleted = null,
        string? priority = null)
    {
        IEnumerable<TodoTask> tasks;

        if (isCompleted.HasValue)
        {
            tasks = await _repository.GetByCompletionStatusAsync(isCompleted.Value);
        }
        else if (!string.IsNullOrEmpty(priority) &&
                 Enum.TryParse<Priority>(priority, true, out var priorityEnum))
        {
            tasks = await _repository.GetByPriorityAsync(priorityEnum);
        }
        else
        {
            tasks = await _repository.GetAllAsync();
        }

        return _mapper.Map<IEnumerable<TodoTaskDto>>(tasks);
    }

    public async Task<TodoTaskDto?> GetTaskByIdAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task == null ? null : _mapper.Map<TodoTaskDto>(task);
    }

    public async Task<TodoTaskDto> CreateTaskAsync(CreateTodoTaskDto createDto)
    {
        var task = _mapper.Map<TodoTask>(createDto);
        var createdTask = await _repository.AddAsync(task);

        _logger.LogInformation("Created task with ID {TaskId}", createdTask.Id);

        return _mapper.Map<TodoTaskDto>(createdTask);
    }

    public async Task<bool> UpdateTaskAsync(Guid id, UpdateTodoTaskDto updateDto)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Attempted to updated non-existent task with ID {TaskId}", id);
            return false;
        }

        _mapper.Map(updateDto, task);
        await _repository.UpdateAsync(task);

        _logger.LogInformation("Task {TaskId} updated successfully ", id);

        return true;
    }

    public async Task<bool> DeleteTaskAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Attempted to delete non-existent task with ID {TaskId}", id);
            return false;
        }

        await _repository.DeleteAsync(task);

        _logger.LogInformation("Task {TaskId} deleted successfully ", id);

        return true;
    }

    public async Task<bool> ToggleTaskCompletionAsync(Guid id)
    {
        var task = await _repository.GetByIdAsync(id);

        if (task == null)
        {
            _logger.LogWarning("Attempted to toggle non-existent task with ID {TaskId}", id);
            return false;
        }

        task.IsCompleted = !task.IsCompleted;
        task.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(task);

        _logger.LogInformation("Task {TaskId} completion toggled to {IsCompleted} ", id, task.IsCompleted);

        return true;
    }
}