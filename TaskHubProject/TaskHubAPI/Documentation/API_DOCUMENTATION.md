# TaskHubAPI - Comprehensive Documentation

TaskHubAPI is a .NET 9 Web API project designed to provide task management capabilities. This project serves as the backend for managing tasks, users, and related operations with a clean architecture pattern.

## Table of Contents
- [Overview](#overview)
- [Architecture](#architecture)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Detailed Code Documentation](#detailed-code-documentation)
  - [Program.cs](#programcs)
  - [Controllers](#controllers)
  - [Models](#models)
  - [DTOs](#dtos)
  - [Data Layer](#data-layer)
- [API Endpoints](#api-endpoints)
- [Usage Examples](#usage-examples)

---

## Overview

TaskHubAPI is a RESTful web service that provides:
- Complete CRUD operations for tasks and users
- In-memory database for development and testing
- Swagger/OpenAPI documentation
- Clean separation of concerns with DTOs
- Enum-based status and priority management

## Architecture

The project follows a layered architecture pattern:

```
TaskHubAPI/
├── Controllers/        # API endpoints and request handling
├── Models/            # Domain entities and enums
├── DTOs/              # Data Transfer Objects
├── Data/              # Database context and configuration
└── Program.cs         # Application entry point
```

**Design Principles:**
- **Separation of Concerns**: Controllers handle HTTP concerns, models represent domain logic
- **DTO Pattern**: Decouples API contracts from domain models
- **Dependency Injection**: DbContext injected into controllers
- **RESTful Design**: Standard HTTP verbs and status codes

## Technologies Used
- **.NET 9** - Latest .NET framework
- **ASP.NET Core Web API** - Web API framework
- **Entity Framework Core (InMemory)** - ORM and in-memory database
- **Swagger/OpenAPI** - API documentation
- **Data Annotations** - Model validation

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Any IDE (Visual Studio, VS Code, Rider)

## Getting Started

### Installation & Running

1. **Navigate to the project directory:**
   ```bash
   cd TaskHubProject/TaskHubAPI
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the project:**
   ```bash
   dotnet build
   ```

4. **Run the API:**
   ```bash
   dotnet run
   ```

5. **Access the API:**
   - HTTPS: `https://localhost:7043`
   - HTTP: `http://localhost:5015`
   - Swagger UI: `https://localhost:7043/swagger`

---

## Detailed Code Documentation

### Program.cs

**Purpose**: Application entry point and configuration

**File Location**: `Program.cs`

**Detailed Explanation**:

```csharp
using Microsoft.EntityFrameworkCore;
using TaskHubAPI.Data;

var builder = WebApplication.CreateBuilder(args);
```
- Creates a `WebApplicationBuilder` instance
- Configures default settings from `appsettings.json`
- Sets up dependency injection container

```csharp
builder.Services.AddControllers();
```
- Registers MVC controller services
- Enables API controller routing and model binding
- Required for `[ApiController]` attribute functionality

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskHubDB"));
```
- **Service Registration**: Adds `AppDbContext` to DI container
- **InMemory Database**: Uses EF Core's in-memory provider
- **Database Name**: "TaskHubDB" - isolated in-memory store
- **Lifetime**: Scoped per HTTP request
- **Note**: Data is lost when application stops (perfect for testing)

```csharp
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
```
- **AddEndpointsApiExplorer**: Discovers API endpoints for OpenAPI
- **AddSwaggerGen**: Configures Swagger documentation generator

```csharp
var app = builder.Build();
```
- Builds the `WebApplication` from configured services
- Creates the HTTP request pipeline

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
```
- **Environment Check**: Only enables Swagger in Development
- **UseSwagger**: Serves OpenAPI specification (JSON)
- **UseSwaggerUI**: Provides interactive documentation interface

```csharp
app.UseHttpsRedirection();
```
- Redirects HTTP requests to HTTPS
- Enforces secure connections

```csharp
app.UseAuthorization();
```
- Adds authorization middleware
- Currently no auth implemented (placeholder for future)

```csharp
app.MapControllers();
```
- Maps attribute-routed controllers
- Discovers `[Route]` and HTTP verb attributes

```csharp
app.MapGet("/", () => Results.Redirect("/swagger"));
```
- Root endpoint redirects to Swagger UI
- Convenience for developers

```csharp
app.Run();
```
- Starts the application
- Blocks until shutdown

---

### Controllers

#### TaskController.cs

**Purpose**: Handles all HTTP requests related to task management

**File Location**: `Controllers/TaskController.cs`

**Class Declaration**:
```csharp
[ApiController]
[Route("api/tasks")]
public class TaskController : ControllerBase
```
- **[ApiController]**: Enables automatic model validation, binding source inference
- **[Route("api/tasks")]**: Base route for all endpoints in this controller
- **ControllerBase**: Base class for API controllers (no view support)

**Dependency Injection**:
```csharp
private readonly AppDbContext _context;
public TaskController(AppDbContext context)
{
    _context = context;
}
```
- **Constructor Injection**: Receives `AppDbContext` from DI container
- **readonly**: Ensures context isn't reassigned
- **Scope**: DbContext is scoped to HTTP request lifecycle

**Endpoints**:

##### 1. Get All Tasks
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<TaskItemDTO>>> GetTasks()
```
- **Route**: `GET /api/tasks`
- **Purpose**: Retrieves all tasks from database
- **Return Type**: `ActionResult<IEnumerable<TaskItemDTO>>`
- **Process**:
  1. Queries all TaskItems from database
  2. Projects to TaskItemDTO using LINQ Select
  3. Converts enums to strings for JSON serialization
  4. Returns 200 OK with task list

**Code Breakdown**:
```csharp
var tasks = await _context.TaskItems.Select(t => new TaskItemDTO
{
    Id = t.Id,
    Title = t.Title,
    Description = t.Description,  
    Status = t.Status.ToString(),      // Enum to string
    Priority = t.Priority.ToString(),  // Enum to string
    DueDate = t.DueDate,
    UserId = t.UserId
}).ToListAsync();
```
- **Select**: Projects entity to DTO (avoids over-fetching)
- **ToString()**: Converts enums to readable strings
- **ToListAsync()**: Executes query asynchronously
- **Why DTOs?**: Prevents exposing navigation properties, controls API shape

##### 2. Get Task by ID
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<TaskItemDTO>> GetTask(int id)
```
- **Route**: `GET /api/tasks/{id}`
- **Parameter**: `id` from URL route
- **Returns**: Single TaskItemDTO or 404 Not Found

**Process**:
```csharp
var t = await _context.TaskItems.FindAsync(id);
if (t == null) return NotFound();
```
- **FindAsync**: Efficient lookup by primary key
- **Null Check**: Returns 404 if task doesn't exist
- **Manual Mapping**: Creates DTO from entity

##### 3. Create Task
```csharp
[HttpPost]
public async Task<ActionResult<TaskItemDTO>> CreateTask(CreateTaskItemDTO dto)
```
- **Route**: `POST /api/tasks`
- **Body**: JSON representing `CreateTaskItemDTO`
- **Returns**: 201 Created with location header

**Validation**:
```csharp
if (!Enum.TryParse<Models.TaskStatus>(dto.Status, out var status) || 
    !Enum.TryParse<TaskPriority>(dto.Priority, out var priority))
    return BadRequest("Invalid status or priority");
```
- **Enum.TryParse**: Safely converts string to enum
- **Validation**: Ensures valid enum values
- **Error Handling**: Returns 400 Bad Request if invalid

**Entity Creation**:
```csharp
var task = new TaskItem
{
    Title = dto.Title,
    Description = dto.Description,
    Status = status,
    Priority = priority,
    DueDate = dto.DueDate,
    UserId = dto.UserId
};
_context.TaskItems.Add(task);
await _context.SaveChangesAsync();
```
- **Mapping**: DTO to entity conversion
- **Add**: Tracks entity for insertion
- **SaveChangesAsync**: Persists to database, generates ID

**Response**:
```csharp
return CreatedAtAction(nameof(GetTask), new { id = task.Id }, taskDTO);
```
- **CreatedAtAction**: Returns 201 Created
- **Location Header**: Points to `GET /api/tasks/{id}`
- **Body**: Contains created task DTO

##### 4. Update Task
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateTask(int id, CreateTaskItemDTO dto)
```
- **Route**: `PUT /api/tasks/{id}`
- **Purpose**: Full update of existing task
- **Returns**: 204 No Content on success

**Process**:
```csharp
var task = await _context.TaskItems.FindAsync(id);
if (task == null) return NotFound();
```
- **Find Existing**: Retrieves task by ID
- **404 Check**: Returns NotFound if doesn't exist

**Update Logic**:
```csharp
task.Title = dto.Title;
task.Description = dto.Description;
task.Status = status;
task.Priority = priority;
task.DueDate = dto.DueDate;
task.UserId = dto.UserId;
await _context.SaveChangesAsync();
```
- **Direct Assignment**: Updates all properties
- **Change Tracking**: EF Core tracks modifications
- **SaveChangesAsync**: Persists changes

##### 5. Delete Task
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteTask(int id)
```
- **Route**: `DELETE /api/tasks/{id}`
- **Returns**: 204 No Content on success

**Process**:
```csharp
var task = await _context.TaskItems.FindAsync(id);
if (task == null) return NotFound();
_context.TaskItems.Remove(task);
await _context.SaveChangesAsync();
```
- **Find**: Locates task to delete
- **Remove**: Marks for deletion
- **SaveChangesAsync**: Executes DELETE SQL

---

#### UserController.cs

**Purpose**: Manages user-related HTTP requests

**File Location**: `Controllers/UserController.cs`

**Similar Structure to TaskController**:

##### 1. Get All Users
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
```
- **Route**: `GET /api/users`
- **Returns**: List of all users as DTOs
- **Projection**: Entity to DTO mapping

##### 2. Get User by ID
```csharp
[HttpGet("{id}")]
public async Task<ActionResult<UserDTO>> GetUser(int id)
```
- **Route**: `GET /api/users/{id}`
- **FindAsync**: Primary key lookup
- **Returns**: UserDTO or 404

##### 3. Create User
```csharp
[HttpPost]
public async Task<ActionResult<UserDTO>> CreateUser(CreateUserDTO dto)
```
- **Route**: `POST /api/users`
- **Validation**: Automatic via Data Annotations
- **Returns**: 201 Created with location

**Validation Notes**:
```csharp
public class CreateUserDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```
- **[ApiController]**: Automatically validates and returns 400
- **ModelState**: Validation errors included in response

##### 4. Update User
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> UpdateUser(int id, CreateUserDTO dto)
```
- **Route**: `PUT /api/users/{id}`
- **Full Update**: Replaces all properties
- **Returns**: 204 No Content

##### 5. Delete User
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
```
- **Route**: `DELETE /api/users/{id}`
- **Note**: No cascade delete implemented
- **Consideration**: Orphaned tasks if user deleted

---

### Models

#### User.cs

**Purpose**: Represents a user entity in the database

**File Location**: `Models/User.cs`

```csharp
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

**Property Details**:

- **Id**:
  - **Type**: `int`
  - **[Key]**: Designates as primary key
  - **Auto-increment**: Database generates value

- **Name**:
  - **Type**: `string`
  - **[Required]**: Cannot be null (database constraint)
  - **Purpose**: User's display name

- **Email**:
  - **Type**: `string`
  - **[Required]**: Mandatory field
  - **[EmailAddress]**: Validates email format
  - **Note**: No uniqueness constraint (could be added)

**Design Considerations**:
- Simple user model (can be extended with password, roles, etc.)
- No navigation property to tasks (can be added for queries)
- Email validation at both DTO and model level

---

#### TaskItem.cs

**Purpose**: Represents a task entity with status tracking

**File Location**: `Models/TaskItem.cs`

**Enums**:

```csharp
public enum TaskStatus
{
    Pending,      // Task not started
    InProgress,   // Task being worked on
    Completed     // Task finished
}

public enum TaskPriority
{
    Low,          // Low priority
    Medium,       // Medium priority
    High          // High priority
}
```
- **Purpose**: Type-safe status and priority values
- **Storage**: Stored as integers in database
- **Benefit**: Prevents invalid values, enables easy validation

**Entity Properties**:

```csharp
[Key]
public int Id { get; set; }
```
- Primary key, auto-generated

```csharp
[Required]
public string Title { get; set; } = string.Empty;
```
- **Required**: Task must have title
- **Default**: Prevents null reference warnings

```csharp
public string? Description { get; set; }
```
- **Nullable**: Optional detailed description
- **?**: C# 9+ nullable reference type

```csharp
[Required]
public TaskStatus Status { get; set; }
```
- **Enum**: One of Pending/InProgress/Completed
- **Required**: Every task must have status

```csharp
[Required]
public TaskPriority Priority { get; set; }
```
- **Enum**: Low/Medium/High
- **Required**: Mandatory prioritization

```csharp
[Required]
public DateTime DueDate { get; set; }
```
- **DateTime**: Task deadline
- **Required**: All tasks have due dates
- **Note**: No time zone handling (consider DateTimeOffset)

```csharp
[ForeignKey("User")]
public int UserId { get; set; }

public User? User { get; set; }
```
- **UserId**: Foreign key to User table
- **[ForeignKey]**: Explicitly names relationship
- **User**: Navigation property (nullable, not always loaded)
- **Relationship**: Many tasks to one user

**Design Considerations**:
- All core properties required (enforces data integrity)
- Enum usage prevents invalid states
- Navigation property allows eager/lazy loading
- No soft delete (consider IsDeleted flag for production)

---

### DTOs

**Purpose**: Data Transfer Objects decouple API contracts from domain models

#### UserDTO.cs

**Purpose**: Represents user data in API responses

**File Location**: `DTOs/UserDTO.cs`

```csharp
public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
```

**Why Use This?**
- **Read Operations**: Safe for GET responses
- **No Validation**: Display-only, no attributes needed
- **Complete Data**: Includes ID for client reference
- **Matches Model**: Simple 1:1 mapping for User entity

---

#### CreateUserDTO.cs

**Purpose**: Accepts user creation data

**File Location**: `DTOs/CreateUserDTO.cs`

```csharp
public class CreateUserDTO
{
    [Required]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }
}
```

**Key Differences from UserDTO**:
- **No Id**: Client doesn't provide ID (server generates)
- **Validation**: [Required] and [EmailAddress] attributes
- **Input DTO**: Used for POST/PUT operations

**Validation Behavior**:
- **[Required]**: Returns 400 if missing
- **[EmailAddress]**: Validates format (e.g., user@example.com)
- **Automatic**: [ApiController] handles validation

---

#### TaskItemDTO.cs

**Purpose**: Task data for API responses

**File Location**: `DTOs/TaskItemDTO.cs`

```csharp
public class TaskItemDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }        // String, not enum
    public string Priority { get; set; }      // String, not enum
    public DateTime DueDate { get; set; }
    public int UserId { get; set; }
}
```

**Design Decisions**:

- **String Enums**:
  - Status/Priority as strings (not enums)
  - Easier JSON serialization
  - Client doesn't need enum knowledge

- **No User Navigation**:
  - Only includes UserId (not full User object)
  - Prevents circular references
  - Client can fetch user separately if needed

- **Includes All Data**:
  - Complete task information
  - Suitable for GET responses

---

#### CreateTaskItemDTO.cs

**Purpose**: Accepts task creation/update data

**File Location**: `DTOs/CreateTaskItemDTO.cs`

```csharp
public class CreateTaskItemDTO
{
    [Required]
    public string Title { get; set; }

    public string Description { get; set; }  // Optional

    [Required]
    public string Status { get; set; }

    [Required]
    public string Priority { get; set; }

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public int UserId { get; set; }
}
```

**Validation Rules**:
- **Title**: Required (task must have name)
- **Description**: Optional (can be null/empty)
- **Status**: Required (must be "Pending", "InProgress", or "Completed")
- **Priority**: Required (must be "Low", "Medium", or "High")
- **DueDate**: Required (all tasks need deadline)
- **UserId**: Required (task must belong to user)

**String Enum Handling**:
- Client sends strings like "Pending" or "High"
- Controller validates and converts to enums
- Invalid values return 400 Bad Request

---

### Data Layer

#### AppDbContext.cs

**Purpose**: Database context for Entity Framework Core

**File Location**: `Data/AppDbContext.cs`

```csharp
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<TaskItem> TaskItems { get; set; }
}
```

**Component Breakdown**:

**Constructor**:
```csharp
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
```
- **Dependency Injection**: Receives configuration from DI
- **Options**: Database provider, connection string, etc.
- **Base Constructor**: Passes options to DbContext

**DbSet Properties**:
```csharp
public DbSet<User> Users { get; set; }
```
- **DbSet**: Represents table in database
- **Users**: Table name will be "Users"
- **CRUD Operations**: Add, Remove, Find methods available

```csharp
public DbSet<TaskItem> TaskItems { get; set; }
```
- **TaskItems**: Table for task entities
- **Relationship**: Foreign key to Users automatically detected

**Database Configuration** (from Program.cs):
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskHubDB"));
```
- **InMemory Provider**: No SQL Server needed
- **Volatile**: Data lost on restart
- **Fast**: Perfect for development/testing

**Entity Framework Features Used**:
- **Change Tracking**: Automatically detects modifications
- **LINQ Support**: Query with C# expressions
- **Async Operations**: All database calls use async/await
- **Relationship Management**: Foreign keys handled automatically

**Potential Enhancements**:
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Configure relationships
    modelBuilder.Entity<TaskItem>()
        .HasOne(t => t.User)
        .WithMany()
        .HasForeignKey(t => t.UserId);
    
    // Add unique constraints
    modelBuilder.Entity<User>()
        .HasIndex(u => u.Email)
        .IsUnique();
    
    // Seed data
    modelBuilder.Entity<User>().HasData(
        new User { Id = 1, Name = "John Doe", Email = "john@example.com" }
    );
}
```

---

## API Endpoints

### Task Endpoints

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/tasks` | Get all tasks | None | Array of TaskItemDTO |
| GET | `/api/tasks/{id}` | Get specific task | None | TaskItemDTO or 404 |
| POST | `/api/tasks` | Create new task | CreateTaskItemDTO | 201 Created + TaskItemDTO |
| PUT | `/api/tasks/{id}` | Update task | CreateTaskItemDTO | 204 No Content |
| DELETE | `/api/tasks/{id}` | Delete task | None | 204 No Content |

### User Endpoints

| Method | Endpoint | Description | Request Body | Response |
|--------|----------|-------------|--------------|----------|
| GET | `/api/users` | Get all users | None | Array of UserDTO |
| GET | `/api/users/{id}` | Get specific user | None | UserDTO or 404 |
| POST | `/api/users` | Create new user | CreateUserDTO | 201 Created + UserDTO |
| PUT | `/api/users/{id}` | Update user | CreateUserDTO | 204 No Content |
| DELETE | `/api/users/{id}` | Delete user | None | 204 No Content |

---

## Usage Examples

### Create a User

**Request**:
```http
POST /api/users
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

**Response**:
```http
HTTP/1.1 201 Created
Location: /api/users/1

{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@example.com"
}
```

### Create a Task

**Request**:
```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Complete project documentation",
  "description": "Write comprehensive docs for TaskHubAPI",
  "status": "InProgress",
  "priority": "High",
  "dueDate": "2025-11-10T23:59:59",
  "userId": 1
}
```

**Response**:
```http
HTTP/1.1 201 Created
Location: /api/tasks/1

{
  "id": 1,
  "title": "Complete project documentation",
  "description": "Write comprehensive docs for TaskHubAPI",
  "status": "InProgress",
  "priority": "High",
  "dueDate": "2025-11-10T23:59:59",
  "userId": 1
}
```

### Get All Tasks

**Request**:
```http
GET /api/tasks
```

**Response**:
```json
[
  {
    "id": 1,
    "title": "Complete project documentation",
    "description": "Write comprehensive docs for TaskHubAPI",
    "status": "InProgress",
    "priority": "High",
    "dueDate": "2025-11-10T23:59:59",
    "userId": 1
  }
]
```

### Update a Task

**Request**:
```http
PUT /api/tasks/1
Content-Type: application/json

{
  "title": "Complete project documentation",
  "description": "Write comprehensive docs for TaskHubAPI",
  "status": "Completed",
  "priority": "High",
  "dueDate": "2025-11-10T23:59:59",
  "userId": 1
}
```

**Response**:
```http
HTTP/1.1 204 No Content
```

### Error Responses

**Invalid Task Status**:
```http
POST /api/tasks
Content-Type: application/json

{
  "title": "Test",
  "status": "InvalidStatus",
  ...
}
```

**Response**:
```http
HTTP/1.1 400 Bad Request

{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Invalid status or priority"
}
```

**Task Not Found**:
```http
GET /api/tasks/999
```

**Response**:
```http
HTTP/1.1 404 Not Found
```

---

## Best Practices Implemented

1. **Async/Await Pattern**: All database operations use async for better scalability
2. **DTO Pattern**: Separates API contracts from domain models
3. **Dependency Injection**: Loose coupling, testable code
4. **RESTful Design**: Standard HTTP verbs and status codes
5. **Validation**: Data annotations and enum validation
6. **Error Handling**: Appropriate HTTP status codes

## Future Enhancements

- [ ] Add authentication and authorization
- [ ] Implement pagination for list endpoints
- [ ] Add filtering and sorting capabilities
- [ ] Replace in-memory database with SQL Server
- [ ] Add unit and integration tests
- [ ] Implement logging (Serilog, NLog)
- [ ] Add CORS configuration
- [ ] Implement soft delete for tasks
- [ ] Add task assignment and collaboration features
- [ ] Email uniqueness constraint

## License
This project is licensed under the MIT License.
