using Microsoft.EntityFrameworkCore;
using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;
using TodoApi.Infrastructure.Data;
using TodoApi.TodoApi.Core.Interfaces;

namespace TodoApi.Infrastructure.Repositories;

public class TodoTaskRepository : Repository<TodoTask>, ITodoTaskRepository
{

    public TodoTaskRepository(TodoDbContext context) : base(context)
    {
    }
    
    public async Task<IEnumerable<TodoTask>> GetByCompletionStatusAsync(bool isCompleted)
    {
        return await _dbSet.Where(x => x.IsCompleted == isCompleted)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TodoTask>> GetByPriorityAsync(Priority priority)
    {
        return await _dbSet.Where(x => x.Priority == priority)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<TodoTask>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .OrderByDescending(x => x.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
    
    public async Task<int> GetTotalCountAsync()
    {
        return await _dbSet.CountAsync();
    }
}