using TodoApi.Core.Entities;
using TodoApi.Core.Entities.Enums;

namespace TodoApi.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(TodoDbContext context)
    {
        if (context.TodoTasks.Any())
            return;

        var tasks = new List<TodoTask>
        {
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Complete project readme file",
                Description = "Actually write a readme file for this project",
                IsCompleted = true,
                Priority = Priority.High,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                DueDate = DateTime.UtcNow.AddDays(-2),
                Tags = "docs,important"
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Implement JWT auth",
                Description = "Add a login and register system with tokens JWT",
                IsCompleted = false,
                Priority = Priority.Urgent,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                DueDate = DateTime.UtcNow.AddDays(2),
                Tags = "backend,security"
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Setup CI/CD pipeline",
                Description = "Implement GitHub Actions for automatic build and deploy",
                IsCompleted = false,
                Priority = Priority.High,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(7),
                Tags = "devops,automation"
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Check code reviews",
                Description = "Check pending pull requests",
                IsCompleted = false,
                Priority = Priority.Medium,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(5),
                Tags = "teamwork,review"
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Update nuget dependencies",
                Description = "Check and update nuget dependencies to their latest version",
                IsCompleted = false,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                Tags = "maintenance"
            },
            new TodoTask
            {
                Id = Guid.NewGuid(),
                Title = "Make project presentation",
                Description = "Make slides to showcase the project",
                IsCompleted = false,
                Priority = Priority.Low,
                CreatedAt = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(3),
                Tags = "presentation,demo"
            },
        };
        
        await context.TodoTasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}