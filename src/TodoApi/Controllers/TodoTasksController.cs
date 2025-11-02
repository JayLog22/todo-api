using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;
using TodoApi.DTOs;
using TodoApi.Services.Interfaces;
using TodoApi.TodoApi.Core.Interfaces;

namespace TodoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoTasksController : ControllerBase
{
    private readonly ITodoTaskService _service;

    public TodoTasksController(ITodoTaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all tasks with optional filters
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetAll([FromQuery] GetAllTodoTaskFilterDto filter)
    {
        var tasks = await _service.GetAllTasksAsync(filter);
        return Ok(tasks);
    }

    /// <summary>
    /// Get a specific task by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoTaskDto>> GetById(Guid id)
    {
        var taskDto = await _service.GetTaskByIdAsync(id);

        if (taskDto == null)
            return NotFound($"Task with id {id} was not found");

        return Ok(taskDto);
    }
    
    /// <summary>
    /// Create a new task
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TodoTaskDto>> Create([FromBody] CreateTodoTaskDto createDto)
    {
        var createdTask = await _service.CreateTaskAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createDto);
    }

    /// <summary>
    /// Update a task
    /// </summary>
    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTodoTaskDto updateDto)
    {
        var success = await _service.UpdateTaskAsync(id, updateDto);

        if (!success)
            return NotFound(new { message = $"Task with id {id} was not found when trying to Update" });

        return NoContent();
    }

    /// <summary>
    /// Delete a specific task
    /// </summary>
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteTaskAsync(id);

        if (!success)
            return NotFound(new { message = $"Task with id {id} was not found when trying to Delete" });

        return NoContent();
    }

    /// <summary>
    /// Toggle task completion status
    /// </summary>
    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult> ToggleComplete(Guid id)
    {
        var success = await _service.ToggleTaskCompletionAsync(id);

        if (!success)
            return NotFound(new
                { message = $"Task with id {id} was not found when trying to Toggle its completion" });

        return NoContent();
    }
}