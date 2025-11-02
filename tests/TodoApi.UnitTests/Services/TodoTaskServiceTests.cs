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
        _service = new TodoTaskService(_repositoryMock.Object, _loggerMock.Object,  _mapperMock.Object);
    }

    #region GetAllTasksAsync Tests

    [Fact]
    public async Task GetAllTasksAsync_NoFilter_ReturnsAllTasks()
    {
        //Arrange
        var filtroDto = new GetAllTodoTaskFilterDto();
        
        var tasks = new List<TodoTask>
        {
            new () {Id = Guid.NewGuid(), Title = "Task 1", Priority = Priority.High },
            new () {Id = Guid.NewGuid(), Title = "Task 2", Priority = Priority.Low },
        };

        var tasksDtos = new List<TodoTaskDto>
        {
            new () {Id = tasks[0].Id, Title = "Task 1", Priority = nameof(Priority.High) },
            new () {Id = tasks[1].Id, Title = "Task 2", Priority = nameof(Priority.Low) },

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
    }

    #endregion
}