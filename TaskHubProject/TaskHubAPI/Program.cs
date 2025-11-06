using Microsoft.EntityFrameworkCore;
using TaskHubAPI.Core.Data;
using TaskHubAPI.DTOs;
using TaskHubAPI.Core.Interfaces;
using TaskHubAPI.Models;
using TaskHubAPI.Core.Repositories;
using TaskHubAPI.Services;
using TaskHubAPI.Core.Strategies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// ============================================
// DEPENDENCY INJECTION CONFIGURATION
// Demonstrates DEPENDENCY INVERSION PRINCIPLE
// ============================================

// Register DbContext with InMemory database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskHubDB"));

// Register Generic Repository Pattern (ABSTRACTION)
// Uses open generic type - works with any entity inheriting BaseEntity
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Business Logic Services (SINGLE RESPONSIBILITY)
// Services depend on repository abstractions (DEPENDENCY INVERSION)
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Register Validation Strategies (STRATEGY PATTERN, POLYMORPHISM)
// Allows different validation implementations to be swapped
builder.Services.AddScoped<IValidationStrategy<CreateUserDTO>, UserValidationStrategy>();
builder.Services.AddScoped<IValidationStrategy<CreateTaskItemDTO>, TaskValidationStrategy>();

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "TaskHub API with OOP Principles",
        Version = "v1",
        Description = @"A RESTful API demonstrating Object-Oriented Programming concepts:
        
        **OOP Principles Implemented:**
        - **Encapsulation**: Private fields, public properties, controlled access
        - **Inheritance**: BaseEntity class extended by User and TaskItem
        - **Polymorphism**: Interface implementations, method overriding, strategy pattern
        - **Abstraction**: Interfaces for repositories and services
        
        **SOLID Principles:**
        - **Single Responsibility**: Each class has one job
        - **Open/Closed**: Open for extension, closed for modification
        - **Liskov Substitution**: Subtypes are substitutable
        - **Interface Segregation**: Specific interfaces for specific needs
        - **Dependency Inversion**: Depend on abstractions, not concretions
        
        **Design Patterns:**
        - Repository Pattern
        - Service Layer Pattern
        - Strategy Pattern
        - Dependency Injection Pattern"
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskHub API v1");
        options.DocumentTitle = "TaskHub API - OOP Implementation";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
