using Microsoft.EntityFrameworkCore;
using TodoApi.Core.Entities;

namespace TodoApi.Infrastructure.Data;

public class TodoDbContext : DbContext
{
    public DbSet<TodoTask> TodoTasks { get; set; }
    
    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TodoDbContext).Assembly);
    }
}