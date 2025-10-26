using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TodoApi.Infrastructure.Data;
using TodoApi.Infrastructure.Repositories;
using TodoApi.Mappings;
using TodoApi.Services.Implementations;
using TodoApi.Services.Interfaces;
using TodoApi.TodoApi.Core.Interfaces;
using FluentValidation;
using Microsoft.OpenApi.Models;
using TodoApi.Converters;
using TodoApi.Filters;

var builder = WebApplication.CreateBuilder(args);

//Add services to container
builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add( new FlexibleDateTimeConverter());
        options.JsonSerializerOptions.Converters.Add( new FlexibleNullableDateTimeConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "To-Do API",
        Version = "v1",
        Description = "A REST API for managing todo tasks"
    });
    
    options.SchemaFilter<SwaggerSchemaFilter>();
});

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

//DbContext setup
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//Register FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddScoped<ValidationFilter>();

//Register repositories
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();

//Register services
builder.Services.AddScoped<ITodoTaskService, TodoTaskService>();

var app = builder.Build();

//Seed DB
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<TodoDbContext>();
        await context.Database.MigrateAsync();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception e)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "An error occurred while seeding the database.");
    }
}

//Http request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
