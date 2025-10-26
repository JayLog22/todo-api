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
    private readonly ILogger<TodoTasksController> _logger;

    public TodoTasksController(ITodoTaskService service, ILogger<TodoTasksController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoTaskDto>>> GetAll([FromQuery] GetAllTodoTaskFilterDto filter)
    {
        try
        {
            var tasks = await _service.GetAllTasksAsync(filter);
            return Ok(tasks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tasks");
            return StatusCode(500, "An error occurred while retrieving tasks");
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TodoTaskDto>> GetById(Guid id)
    {
        try
        {
            var taskDto = await _service.GetTaskByIdAsync(id);

            if (taskDto == null)
                return NotFound($"Task with id {id} was not found");

            return Ok(taskDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving task {TaskId}", id);
            return StatusCode(500, "An error occurred while retrieving task");
        }
    }

    [HttpPost]
    public async Task<ActionResult<TodoTaskDto>> Create([FromBody] CreateTodoTaskDto createDto)
    {
        try
        {
            var createdTask = await _service.CreateTaskAsync(createDto);
            return CreatedAtAction(nameof(GetById), new { id = createdTask.Id }, createDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating task");
            return StatusCode(500, "An error occurred while creating task");
        }
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult> Update(Guid id, [FromBody] UpdateTodoTaskDto updateDto)
    {
        try
        {
            var success = await _service.UpdateTaskAsync(id, updateDto);

            if (!success)
                return NotFound(new {message = $"Task with id {id} was not found when trying to Update"});
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId}", id);
            return StatusCode(500, "An error occurred while updating the task");
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var success = await _service.DeleteTaskAsync(id);

            if (!success)
                return NotFound(new {message = $"Task with id {id} was not found when trying to Delete"});
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting task {TaskId}", id);
            return StatusCode(500, "An error occurred while deleting the task");
        }
    }

    [HttpPatch("{id:guid}/toggle")]
    public async Task<ActionResult> ToggleComplete(Guid id)
    {
        try
        {
            var success = await _service.ToggleTaskCompletionAsync(id);

            if (!success)
                return NotFound(new {message = $"Task with id {id} was not found when trying to Toggle its completion"});

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling task {TaskId}", id);
            return StatusCode(500, "An error occurred while toggling the task");
        }
    }
}