using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;

namespace TodoApi.TodoApi.Core.Interfaces;

public interface ITodoTaskRepository : IRepository<TodoTask>
{
    Task<IEnumerable<TodoTask>> GetByCompletionStatusAsync(bool isCompleted);
    Task<IEnumerable<TodoTask>> GetByPriorityAsync(Priority priority);
    Task<IEnumerable<TodoTask>> GetPagedAsync(int pageNumber, int pageSize);
    Task<int> GetTotalCountAsync();
}