using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;
using TodoApi.DTOs;
using TodoApi.Services.Implementations;
using TodoApi.TodoApi.Core.Interfaces;

namespace TodoApi.UnitTests.Services;

public class TodoTaskServiceTests
{
    private readonly Mock<ITodoTaskRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<TodoTaskService>> _loggerMock;
    private readonly TodoTaskService _service;

    public TodoTaskServiceTests()
    {
        _repositoryMock = new Mock<ITodoTaskRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<TodoTaskService>>();
        _service = new TodoTaskService(_repositoryMock.Object, _loggerMock.Object, _mapperMock.Object);
    }

    #region GetAllTasksAsync Tests

    [Fact]
    public async Task GetAllTasksAsync_NoFilter_ReturnsAllTasks()
    {
        //Arrange
        var filtroDto = new GetAllTodoTaskFilterDto();

        var tasks = new List<TodoTask>
        {
            new() { Id = Guid.NewGuid(), Title = "Task 1", Priority = Priority.High },
            new() { Id = Guid.NewGuid(), Title = "Task 2", Priority = Priority.Low },
        };

        var tasksDtos = new List<TodoTaskDto>
        {
            new() { Id = tasks[0].Id, Title = "Task 1", Priority = nameof(Priority.High) },
            new() { Id = tasks[1].Id, Title = "Task 2", Priority = nameof(Priority.Low) },
        };

        _repositoryMock
            .Setup(r => r.GetAllAsync()).ReturnsAsync(tasks);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TodoTaskDto>>(It.IsAny<IEnumerable<TodoTask>>()))
            .Returns(tasksDtos);

        //Act
        var result = await _service.GetAllTasksAsync(filtroDto);

        //Assert
        var todoTaskDtos = result.ToList();
        todoTaskDtos.Should().HaveCount(2);
        todoTaskDtos.Should().BeEquivalentTo(tasksDtos);

        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        _repositoryMock.Verify(r => r.GetByCompletionStatusAsync(It.IsAny<bool>()), Times.Never);
        _repositoryMock.Verify(r => r.GetByPriorityAsync(It.IsAny<Priority>()), Times.Never);
    }
    [Fact]
    public async Task GetAllTasksAsync_WithIsCompletedFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filterDto = new GetAllTodoTaskFilterDto { IsCompleted = true };

        var completedTasks = new List<TodoTask>
        {
            new() { Id = Guid.NewGuid(), Title = "Completed Task", IsCompleted = true }
        };

        var taskDtos = new List<TodoTaskDto>
        {
            new() { Id = completedTasks[0].Id, Title = "Completed Task", IsCompleted = true }
        };

        _repositoryMock
            .Setup(r => r.GetByCompletionStatusAsync(true))
            .ReturnsAsync(completedTasks);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TodoTaskDto>>(It.IsAny<IEnumerable<TodoTask>>()))
            .Returns(taskDtos);

        // Act
        var result = await _service.GetAllTasksAsync(filterDto);

        // Assert
        result.Should().HaveCount(1);
        result.First().IsCompleted.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByCompletionStatusAsync(true), Times.Once);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllTasksAsync_WithPriorityFilter_ReturnsFilteredTasks()
    {
        // Arrange
        var filterDto = new GetAllTodoTaskFilterDto { Priority = Priority.Urgent };

        var urgentTasks = new List<TodoTask>
        {
            new() { Id = Guid.NewGuid(), Title = "Urgent Task", Priority = Priority.Urgent }
        };

        var taskDtos = new List<TodoTaskDto>
        {
            new() { Id = urgentTasks[0].Id, Title = "Urgent Task", Priority = "Urgent" }
        };

        _repositoryMock
            .Setup(r => r.GetByPriorityAsync(Priority.Urgent))
            .ReturnsAsync(urgentTasks);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TodoTaskDto>>(It.IsAny<IEnumerable<TodoTask>>()))
            .Returns(taskDtos);

        // Act
        var result = await _service.GetAllTasksAsync(filterDto);

        // Assert
        result.Should().HaveCount(1);
        result.First().Priority.Should().Be("Urgent");

        _repositoryMock.Verify(r => r.GetByPriorityAsync(Priority.Urgent), Times.Once);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllTasksAsync_WithBothFilters_PrioritizesIsCompleted()
    {
        // Arrange
        var filterDto = new GetAllTodoTaskFilterDto
        {
            IsCompleted = false,
            Priority = Priority.High
        };

        var incompleteTasks = new List<TodoTask>
        {
            new() { Id = Guid.NewGuid(), Title = "Incomplete", IsCompleted = false, Priority = Priority.High }
        };

        var taskDtos = new List<TodoTaskDto>
        {
            new() { Id = incompleteTasks[0].Id, Title = "Incomplete", IsCompleted = false }
        };

        _repositoryMock
            .Setup(r => r.GetByCompletionStatusAsync(false))
            .ReturnsAsync(incompleteTasks);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<TodoTaskDto>>(It.IsAny<IEnumerable<TodoTask>>()))
            .Returns(taskDtos);

        // Act
        var result = await _service.GetAllTasksAsync(filterDto);

        // Assert
        result.Should().HaveCount(1);
        _repositoryMock.Verify(r => r.GetByCompletionStatusAsync(false), Times.Once);
        _repositoryMock.Verify(r => r.GetByPriorityAsync(It.IsAny<Priority>()), Times.Never);
    }

    #endregion

    #region GetTaskByIdAsync Tests

    [Fact]
    public async Task GetTaskByIdAsync_ExistingId_ReturnsTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TodoTask { Id = taskId, Title = "Test Task", Priority = Priority.Medium };
        var taskDto = new TodoTaskDto { Id = taskId, Title = "Test Task", Priority = "Medium" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        _mapperMock
            .Setup(m => m.Map<TodoTaskDto>(It.IsAny<TodoTask>()))
            .Returns(taskDto);

        // Act
        var result = await _service.GetTaskByIdAsync(taskId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(taskId);
        result.Title.Should().Be("Test Task");
        result.Priority.Should().Be("Medium");

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mapperMock.Verify(m => m.Map<TodoTaskDto>(It.IsAny<TodoTask>()), Times.Once);
    }

    [Fact]
    public async Task GetTaskByIdAsync_NonExistingId_ReturnsNull()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TodoTask?)null);

        // Act
        var result = await _service.GetTaskByIdAsync(taskId);

        // Assert
        result.Should().BeNull();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _mapperMock.Verify(m => m.Map<TodoTaskDto>(It.IsAny<TodoTask>()), Times.Never);
    }

    #endregion

    #region CreateTaskAsync Tests

    [Fact]
    public async Task CreateTaskAsync_ValidDto_ReturnsCreatedTask()
    {
        // Arrange
        var createDto = new CreateTodoTaskDto
        {
            Title = "New Task",
            Description = "Task description",
            Priority = "High"
        };

        var task = new TodoTask
        {
            Id = Guid.NewGuid(),
            Title = "New Task",
            Description = "Task description",
            Priority = Priority.High,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        var taskDto = new TodoTaskDto
        {
            Id = task.Id,
            Title = "New Task",
            Description = "Task description",
            Priority = "High",
            IsCompleted = false
        };

        _mapperMock
            .Setup(m => m.Map<TodoTask>(It.IsAny<CreateTodoTaskDto>()))
            .Returns(task);

        _repositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TodoTask>()))
            .ReturnsAsync(task);

        _mapperMock
            .Setup(m => m.Map<TodoTaskDto>(It.IsAny<TodoTask>()))
            .Returns(taskDto);

        // Act
        var result = await _service.CreateTaskAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("New Task");
        result.Priority.Should().Be("High");
        result.IsCompleted.Should().BeFalse();

        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<TodoTask>()), Times.Once);
    }

    #endregion

    #region PatchTaskAsync Tests

    [Fact]
    public async Task PatchTaskAsync_ExistingTask_UpdatesAndReturnsTrue()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var existingTask = new TodoTask
        {
            Id = taskId,
            Title = "Original Title",
            Description = "Original Description",
            Priority = Priority.Low
        };

        var patchDto = new UpdateTodoTaskDto()
        {
            Description = "Updated Description"
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(existingTask);

        _mapperMock
            .Setup(m => m.Map(It.IsAny<UpdateTodoTaskDto>(), It.IsAny<TodoTask>()))
            .Callback<UpdateTodoTaskDto, TodoTask>((dto, task) =>
            {
                task.Description = dto.Description ?? task.Description;
                task.UpdatedAt = DateTime.UtcNow;
            });

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.UpdateTaskAsync(taskId, patchDto);

        // Assert
        result.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TodoTask>()), Times.Once);
    }

    [Fact]
    public async Task PatchTaskAsync_NonExistingTask_ReturnsFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var patchDto = new UpdateTodoTaskDto { Title = "Updated Title" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TodoTask?)null);

        // Act
        var result = await _service.UpdateTaskAsync(taskId, patchDto);

        // Assert
        result.Should().BeFalse();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TodoTask>()), Times.Never);
        _mapperMock.Verify(m => m.Map(It.IsAny<UpdateTodoTaskDto>(), It.IsAny<TodoTask>()), Times.Never);
    }

    #endregion

    #region DeleteTaskAsync Tests

    [Fact]
    public async Task DeleteTaskAsync_ExistingTask_DeletesAndReturnsTrue()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TodoTask { Id = taskId, Title = "Task to Delete" };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        _repositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteTaskAsync(taskId);

        // Assert
        result.Should().BeTrue();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(task), Times.Once);
    }

    [Fact]
    public async Task DeleteTaskAsync_NonExistingTask_ReturnsFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TodoTask?)null);

        // Act
        var result = await _service.DeleteTaskAsync(taskId);

        // Assert
        result.Should().BeFalse();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _repositoryMock.Verify(r => r.DeleteAsync(It.IsAny<TodoTask>()), Times.Never);
    }

    #endregion

    #region ToggleTaskCompletionAsync Tests

    [Fact]
    public async Task ToggleTaskCompletionAsync_ExistingTask_TogglesFromFalseToTrue()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TodoTask
        {
            Id = taskId,
            Title = "Task",
            IsCompleted = false
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ToggleTaskCompletionAsync(taskId);

        // Assert
        result.Should().BeTrue();
        task.IsCompleted.Should().BeTrue();

        _repositoryMock.Verify(r => r.UpdateAsync(task), Times.Once);
    }

    [Fact]
    public async Task ToggleTaskCompletionAsync_ExistingTask_TogglesFromTrueToFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TodoTask
        {
            Id = taskId,
            Title = "Task",
            IsCompleted = true
        };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync(task);

        _repositoryMock
            .Setup(r => r.UpdateAsync(It.IsAny<TodoTask>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.ToggleTaskCompletionAsync(taskId);

        // Assert
        result.Should().BeTrue();
        task.IsCompleted.Should().BeFalse();

        _repositoryMock.Verify(r => r.UpdateAsync(task), Times.Once);
    }

    [Fact]
    public async Task ToggleTaskCompletionAsync_NonExistingTask_ReturnsFalse()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(taskId))
            .ReturnsAsync((TodoTask?)null);

        // Act
        var result = await _service.ToggleTaskCompletionAsync(taskId);

        // Assert
        result.Should().BeFalse();

        _repositoryMock.Verify(r => r.GetByIdAsync(taskId), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TodoTask>()), Times.Never);
    }

    #endregion
}