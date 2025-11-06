# TaskHubAPI - Complete Project Guide

**TaskHubAPI** is a .NET 9 Web API project showcasing comprehensive **Object-Oriented Programming (OOP)** concepts, SOLID principles, and design patterns in a production-ready architecture.

---

## 📚 Table of Contents
- [Project Overview](#project-overview)
- [Quick Start](#quick-start)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Key Features](#key-features)
- [API Endpoints](#api-endpoints)
- [Configuration](#configuration)
- [How to Run](#how-to-run)
- [Documentation Files](#documentation-files)
- [Technology Stack](#technology-stack)

---

## Project Overview

TaskHubAPI is a RESTful API for managing users and tasks with a focus on demonstrating professional software development practices:

- **Framework**: .NET 9 (Latest)
- **Database**: Entity Framework Core with InMemory provider
- **Architecture**: Clean Layered Architecture
- **OOP**: All 4 pillars implemented (Encapsulation, Inheritance, Polymorphism, Abstraction)
- **SOLID**: All 5 principles applied throughout
- **Patterns**: Repository, Service Layer, Strategy, Dependency Injection

---

## Quick Start

### Prerequisites
- .NET 9 SDK installed
- Visual Studio 2022 or VS Code
- Postman or similar API testing tool (optional)

### Running the Application

```powershell
# Navigate to project directory
cd TaskHubAPI

# Restore dependencies
dotnet restore

# Run the application
dotnet run

# Application will start at:
# https://localhost:5001 (HTTPS)
# http://localhost:5000 (HTTP)
```

### Test the API

```powershell
# Create a user
curl -X POST https://localhost:5001/api/users -H "Content-Type: application/json" -d '{\"name\":\"John Doe\",\"email\":\"john@example.com\"}'

# Get all users
curl https://localhost:5001/api/users

# Create a task
curl -X POST https://localhost:5001/api/tasks -H "Content-Type: application/json" -d '{\"title\":\"Complete Project\",\"dueDate\":\"2025-12-31\",\"userId\":1}'
```

---

## Project Structure

```
TaskHubAPI/
│
├── 📂 Controllers/                      # HTTP Request Handlers (Presentation Layer)
│   ├── UserController.cs               # User endpoints - CRUD operations
│   └── TaskController.cs               # Task endpoints - CRUD + filtering
│
├── 📂 Models/                           # Domain Entities (OOP: Inheritance)
│   ├── BaseEntity.cs                   # Abstract base class with common properties
│   ├── User.cs                         # User entity (inherits BaseEntity)
│   └── TaskItem.cs                     # Task entity (inherits BaseEntity)
│
├── 📂 Services/                         # Business Logic Layer (Service Pattern)
│   ├── UserService.cs                  # User business logic & validation
│   └── TaskService.cs                  # Task business logic & filtering
│
├── 📂 DTOs/                             # Data Transfer Objects
│   ├── UserDTO.cs                      # User response DTO
│   ├── CreateUserDTO.cs                # User creation/update DTO
│   ├── TaskItemDTO.cs                  # Task response DTO
│   └── CreateTaskItemDTO.cs            # Task creation/update DTO
│
├── 📂 Core/                             # Core Infrastructure Layer
│   ├── 📂 Interfaces/                  # Abstraction Layer (OOP: Abstraction)
│   │   ├── IRepository.cs              # Generic repository contract
│   │   ├── IUserService.cs             # User service contract
│   │   ├── ITaskService.cs             # Task service contract
│   │   └── IValidationStrategy.cs      # Validation strategy contract
│   ├── 📂 Repositories/                # Data Access Layer (Repository Pattern)
│   │   └── Repository.cs               # Generic repository implementation
│   ├── 📂 Strategies/                  # Strategy Pattern (OOP: Polymorphism)
│   │   └── ValidationStrategies.cs     # Validation implementations
│   └── 📂 Data/                        # Database Context
│       └── AppDbContext.cs             # EF Core DbContext
│
├── 📂 Configuration/                    # Application Configuration
│   ├── appsettings.json                # Production settings
│   └── appsettings.Development.json    # Development settings
│
├── 📂 Documentation/                    # Project Documentation
│   ├── PROJECT_GUIDE.md                # This file - Complete project guide
│   ├── API_DOCUMENTATION.md            # Detailed API reference
│   └── OOP_DOCUMENTATION.md            # OOP concepts & implementation
│
├── 📂 Properties/                       # Launch Configuration
│   └── launchSettings.json             # Debug/launch profiles
│
├── 📄 Program.cs                        # ⭐ Application Entry Point & DI Setup
├── 📄 README.md                         # Project overview & quick links
├── 📄 TaskHubAPI.csproj                 # Project configuration file
├── 📄 TaskHubAPI.sln                    # Visual Studio solution file
└── 📄 TaskHubAPI.http                   # HTTP request examples
```

### 📝 Folder Organization Benefits

**Core/** folder consolidates infrastructure components:
- **Interfaces/** - All abstraction contracts in one place
- **Repositories/** - Data access implementations
- **Strategies/** - Pattern implementations (Strategy, etc.)
- **Data/** - Database context and configuration

This structure provides:
✅ **Reduced complexity** - Fewer top-level folders (7 main folders vs 11 previously)
✅ **Logical grouping** - Related infrastructure code in Core/
✅ **Clear separation** - Business logic (Services/) separate from infrastructure (Core/)
✅ **Better organization** - Easy to find related files
│
├── 📂 Services/                         # Business Logic Layer (Service Pattern)
│   ├── UserService.cs                  # User business logic & validation
│   └── TaskService.cs                  # Task business logic & filtering
│
├── 📂 Strategies/                       # Strategy Pattern (OOP: Polymorphism)
│   └── ValidationStrategies.cs         # Validation implementations
│
├── 📂 Data/                             # Database Context
│   └── AppDbContext.cs                 # EF Core DbContext
│
├── 📂 Configuration/                    # Application Configuration
│   ├── appsettings.json                # Production settings
│   └── appsettings.Development.json    # Development settings
│
├── 📂 Documentation/                    # Project Documentation
│   ├── PROJECT_GUIDE.md                # This file - Complete project guide
│   ├── API_DOCUMENTATION.md            # Detailed API reference
│   └── OOP_DOCUMENTATION.md            # OOP concepts & implementation
│
├── 📂 Properties/
│   └── launchSettings.json             # Launch configuration
│
└── 📄 Program.cs                        # Application entry point & DI setup
```

---

## Architecture

### Layered Architecture Overview

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                       │
│  Controllers: UserController, TaskController                │
│  Responsibility: HTTP handling, routing, error responses    │
└─────────────────┬───────────────────────────────────────────┘
                  │ Depends on IUserService, ITaskService
┌─────────────────▼───────────────────────────────────────────┐
│                   BUSINESS LOGIC LAYER                      │
│  Services: UserService, TaskService                         │
│  Responsibility: Business rules, validation, DTO mapping    │
└─────────────────┬───────────────────────────────────────────┘
                  │ Depends on IRepository<T>
┌─────────────────▼───────────────────────────────────────────┐
│                    DATA ACCESS LAYER                        │
│  Repositories: Repository<T>                                │
│  Responsibility: Database operations (CRUD)                 │
└─────────────────┬───────────────────────────────────────────┘
                  │ Uses AppDbContext
┌─────────────────▼───────────────────────────────────────────┐
│                    DATABASE CONTEXT                         │
│  AppDbContext: EF Core DbContext                            │
│  DbSet<User>, DbSet<TaskItem>                               │
└─────────────────┬───────────────────────────────────────────┘
                  │ Maps to
┌─────────────────▼───────────────────────────────────────────┐
│                     DOMAIN MODELS                           │
│  BaseEntity (abstract) → User, TaskItem                     │
│  Responsibility: Business entities & validation             │
└─────────────────────────────────────────────────────────────┘
```

### Request Flow Example

```
1. HTTP POST /api/users
   ↓
2. UserController.CreateUser(CreateUserDTO)
   ↓ Validates using IValidationStrategy
3. IUserService.CreateUserAsync(CreateUserDTO)
   ↓ Business rules (email uniqueness)
4. IRepository<User>.AddAsync(User)
   ↓ Database operation
5. AppDbContext.Users.Add(user)
   ↓
6. Returns UserDTO → Controller → HTTP 201 Created
```

---

## Key Features

### OOP Concepts Implemented

#### 1. 🔒 Encapsulation
- **Private fields** with **public properties**
- **Automatic data normalization** (email lowercase, trim)
- **Controlled access** to entity state
- **Files**: `Models/User.cs`, `Models/TaskItem.cs`

```csharp
// Example: User.cs
private string _email = string.Empty;
public string Email
{
    get => _email;
    set => _email = value?.Trim().ToLowerInvariant() ?? string.Empty;
}
```

#### 2. 🧬 Inheritance
- **BaseEntity** abstract class
- **Common properties**: Id, CreatedAt, UpdatedAt
- **Code reuse** across all entities
- **Files**: `Models/BaseEntity.cs` → `User.cs`, `TaskItem.cs`

```csharp
public abstract class BaseEntity { /* Id, CreatedAt, UpdatedAt */ }
public class User : BaseEntity { /* Name, Email */ }
public class TaskItem : BaseEntity { /* Title, Status, Priority */ }
```

#### 3. 🎭 Polymorphism
- **Method overriding**: Validate() method
- **Interface implementations**: Multiple strategies
- **Runtime behavior**: Different validation for User vs TaskItem
- **Files**: All service and strategy implementations

```csharp
BaseEntity entity = new User("John", "john@test.com");
bool isValid = entity.Validate(); // Calls User.Validate()
```

#### 4. 🎨 Abstraction
- **Interface-based design**: IRepository, IUserService, ITaskService
- **Hide complexity**: Controllers don't know about database
- **Dependency Inversion**: Depend on abstractions, not implementations
- **Files**: All files in `Interfaces/` folder

### SOLID Principles Applied

| Principle | Implementation | Benefit |
|-----------|---------------|---------|
| **S**ingle Responsibility | Each class has one job (Controller→Service→Repository) | Easy to maintain |
| **O**pen/Closed | Can add new entities/strategies without modifying existing code | Extensible |
| **L**iskov Substitution | User/TaskItem can replace BaseEntity anywhere | Polymorphic |
| **I**nterface Segregation | Small, focused interfaces (IRepository, IValidationStrategy) | No fat interfaces |
| **D**ependency Inversion | All dependencies injected via interfaces | Loose coupling |

### Design Patterns

1. **Repository Pattern**
   - Centralized data access
   - Database-agnostic operations
   - Easy to test with mocks

2. **Service Layer Pattern**
   - Business logic separation
   - Reusable across controllers
   - Independent testing

3. **Strategy Pattern**
   - Interchangeable validation algorithms
   - Runtime strategy selection
   - Easy to add new strategies

4. **Dependency Injection**
   - Constructor injection throughout
   - Configured in `Program.cs`
   - IoC container manages lifetime

---

## API Endpoints

### User Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| GET | `/api/users` | Get all users | None |
| GET | `/api/users/{id}` | Get user by ID | None |
| GET | `/api/users/email/{email}` | Get user by email | None |
| POST | `/api/users` | Create new user | `CreateUserDTO` |
| PUT | `/api/users/{id}` | Update user | `CreateUserDTO` |
| DELETE | `/api/users/{id}` | Delete user | None |

### Task Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| GET | `/api/tasks` | Get all tasks | None |
| GET | `/api/tasks/{id}` | Get task by ID | None |
| GET | `/api/tasks/user/{userId}` | Get tasks by user | None |
| GET | `/api/tasks/status/{status}` | Get tasks by status | None |
| GET | `/api/tasks/priority/{priority}` | Get tasks by priority | None |
| GET | `/api/tasks/overdue` | Get overdue tasks | None |
| POST | `/api/tasks` | Create new task | `CreateTaskItemDTO` |
| PUT | `/api/tasks/{id}` | Update task | `CreateTaskItemDTO` |
| DELETE | `/api/tasks/{id}` | Delete task | None |

**📖 See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for detailed request/response examples.**

---

## Configuration

### appsettings.json

Located in `Configuration/appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### appsettings.Development.json

Located in `Configuration/appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### Database Configuration

Currently using **Entity Framework Core InMemory** database for development:

```csharp
// Program.cs
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TaskHubDB"));
```

**To switch to SQL Server:**

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

---

## How to Run

### Development Mode

```powershell
# Run with development settings
dotnet run --environment Development

# Run with hot reload (auto-restart on file changes)
dotnet watch run
```

### Production Mode

```powershell
# Build for release
dotnet build -c Release

# Run release build
dotnet run -c Release
```

### Using Visual Studio

1. Open `TaskHubAPI.sln`
2. Press `F5` to run with debugging
3. Press `Ctrl+F5` to run without debugging

### Testing Endpoints

**Using Swagger UI:**
- Navigate to `https://localhost:5001/swagger`
- Interactive API documentation
- Test endpoints directly in browser

**Using PowerShell:**

```powershell
# Create user
$body = @{
    name = "John Doe"
    email = "john@example.com"
} | ConvertTo-Json

Invoke-WebRequest -Uri "https://localhost:5001/api/users" -Method POST -Body $body -ContentType "application/json"

# Get all users
Invoke-WebRequest -Uri "https://localhost:5001/api/users" -Method GET
```

---

## Documentation Files

| File | Purpose | Lines | Details |
|------|---------|-------|---------|
| **PROJECT_GUIDE.md** | Complete project overview | ~600 | This file - structure, setup, features |
| **API_DOCUMENTATION.md** | API reference | ~980 | Endpoints, request/response examples |
| **OOP_DOCUMENTATION.md** | OOP concepts | ~880 | OOP pillars, SOLID, patterns, examples |

### Quick Navigation

- **New to project?** → Start with this file (PROJECT_GUIDE.md)
- **Need API details?** → See [API_DOCUMENTATION.md](API_DOCUMENTATION.md)
- **Learning OOP?** → See [OOP_DOCUMENTATION.md](OOP_DOCUMENTATION.md)

---

## Technology Stack

### Core Technologies
- **.NET 9** - Latest .NET framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9** - ORM for database access
- **C# 13** - Programming language

### Database
- **EF Core InMemory** - Development database
- Can switch to SQL Server, PostgreSQL, MySQL, etc.

### Development Tools
- **Visual Studio 2022** or **VS Code**
- **Swagger/OpenAPI** - API documentation
- **Postman** - API testing (optional)

### NuGet Packages
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="9.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

---

## Project Statistics

| Category | Count | Lines of Code |
|----------|-------|---------------|
| Controllers | 2 | ~350 |
| Models | 3 | ~260 |
| Interfaces | 4 | ~220 |
| Repositories | 1 | ~120 |
| Services | 2 | ~380 |
| Strategies | 1 | ~160 |
| DTOs | 4 | ~65 |
| Data Context | 1 | ~15 |
| **Total Code** | **18 files** | **~1,570 lines** |
| Configuration | 2 | ~50 |
| Documentation | 3 | ~2,500+ |

---

## Folder Descriptions

### Controllers/
**Purpose**: HTTP request handling and routing  
**Responsibility**: Receives HTTP requests, validates input, calls services, returns responses  
**Dependencies**: IUserService, ITaskService, IValidationStrategy  
**Files**: UserController.cs, TaskController.cs

### Models/
**Purpose**: Domain entities representing business objects  
**Responsibility**: Entity definitions, business logic, validation  
**OOP Features**: Inheritance (BaseEntity), Encapsulation (private fields), Polymorphism (Validate override)  
**Files**: BaseEntity.cs, User.cs, TaskItem.cs

### DTOs/
**Purpose**: Data Transfer Objects for API requests/responses  
**Responsibility**: Shape data for external consumption, decouple API from domain  
**Pattern**: DTO Pattern  
**Files**: UserDTO.cs, CreateUserDTO.cs, TaskItemDTO.cs, CreateTaskItemDTO.cs

### Interfaces/
**Purpose**: Contracts and abstractions  
**Responsibility**: Define behavior without implementation  
**OOP Features**: Abstraction, Dependency Inversion  
**Files**: IRepository.cs, IUserService.cs, ITaskService.cs, IValidationStrategy.cs

### Repositories/
**Purpose**: Data access layer  
**Responsibility**: CRUD operations on database  
**Pattern**: Repository Pattern  
**Files**: Repository.cs (generic implementation for all entities)

### Services/
**Purpose**: Business logic layer  
**Responsibility**: Business rules, validation, DTO mapping  
**Pattern**: Service Layer Pattern  
**Files**: UserService.cs, TaskService.cs

### Strategies/
**Purpose**: Validation implementations  
**Responsibility**: Validation logic for different entity types  
**Pattern**: Strategy Pattern  
**Files**: ValidationStrategies.cs (UserValidationStrategy, TaskValidationStrategy, CompositeValidationStrategy)

### Data/
**Purpose**: Database context  
**Responsibility**: EF Core configuration, DbSets  
**Files**: AppDbContext.cs

### Configuration/
**Purpose**: Application settings  
**Responsibility**: JSON configuration files  
**Files**: appsettings.json, appsettings.Development.json

### Documentation/
**Purpose**: Project documentation  
**Responsibility**: Guides, references, explanations  
**Files**: PROJECT_GUIDE.md (this file), API_DOCUMENTATION.md, OOP_DOCUMENTATION.md

---

## Common Tasks

### Adding a New Entity

1. Create entity in `Models/` inheriting `BaseEntity`
2. Add DbSet to `AppDbContext.cs`
3. Create DTOs in `DTOs/` folder
4. Create service interface in `Interfaces/`
5. Implement service in `Services/`
6. Create controller in `Controllers/`
7. Register service in `Program.cs`

**Example:**
```csharp
// 1. Model
public class Project : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public override bool Validate() => !string.IsNullOrWhiteSpace(Name);
}

// 2. DbContext
public DbSet<Project> Projects => Set<Project>();

// 7. Program.cs
builder.Services.AddScoped<IRepository<Project>, Repository<Project>>();
```

### Adding a New Validation Strategy

```csharp
public class EmailDomainValidationStrategy : IValidationStrategy<CreateUserDTO>
{
    public ValidationResult Validate(CreateUserDTO user)
    {
        var result = new ValidationResult();
        if (!user.Email.EndsWith("@company.com"))
            result.AddError("Email must be from company domain");
        return result;
    }
}

// Register in Program.cs or use composite strategy
var composite = new CompositeValidationStrategy<CreateUserDTO>();
composite.AddStrategy(new UserValidationStrategy());
composite.AddStrategy(new EmailDomainValidationStrategy());
```

### Switching to SQL Server

1. Install package: `dotnet add package Microsoft.EntityFrameworkCore.SqlServer`
2. Update `Program.cs`:
```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```
3. Add connection string to `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskHubDB;Trusted_Connection=true;"
}
```
4. Create migration: `dotnet ef migrations add Initial`
5. Update database: `dotnet ef database update`

---

## Best Practices Demonstrated

✅ **Clean Architecture** - Clear separation of concerns  
✅ **Interface-Based Design** - Program to interfaces, not implementations  
✅ **Dependency Injection** - Loose coupling throughout  
✅ **Generic Programming** - Repository<T> works with any entity  
✅ **DTO Pattern** - API contracts separate from domain  
✅ **Async/Await** - Non-blocking I/O operations  
✅ **Validation** - Multiple layers (DTO, domain, business)  
✅ **Error Handling** - Proper HTTP status codes  
✅ **Documentation** - Comprehensive inline and external docs  
✅ **Naming Conventions** - Clear, consistent naming  

---

## Troubleshooting

### Port Already in Use

```powershell
# Change ports in Properties/launchSettings.json
"applicationUrl": "https://localhost:5002;http://localhost:5001"
```

### Database Not Persisting

**Issue**: InMemory database resets on restart  
**Solution**: Switch to SQL Server or SQLite for persistent storage

### Swagger Not Loading

**Check**: Navigate to `/swagger/index.html`  
**Ensure**: `builder.Services.AddSwaggerGen()` is in Program.cs

---

## Learning Resources

### Understanding OOP in This Project

1. **Start Here**: Read `Models/BaseEntity.cs` - See inheritance in action
2. **Next**: Read `Models/User.cs` - See encapsulation with private fields
3. **Then**: Read `Interfaces/IRepository.cs` - See abstraction
4. **Finally**: Read `Services/UserService.cs` - See all concepts together

### Understanding Flow

1. **HTTP Request** → `Controllers/UserController.cs` (GET /api/users)
2. **Service Call** → `Services/UserService.cs` (GetAllUsersAsync)
3. **Repository Call** → `Repositories/Repository.cs` (GetAllAsync)
4. **Database** → `Data/AppDbContext.cs` (EF Core queries)
5. **Return Flow** → Repository → Service → Controller → HTTP Response

---

## Future Enhancements

- [ ] Authentication & Authorization (JWT)
- [ ] Advanced filtering & pagination
- [ ] Caching layer (Redis)
- [ ] Logging & monitoring (Serilog)
- [ ] Unit & integration tests
- [ ] API versioning
- [ ] Rate limiting
- [ ] Health checks
- [ ] Docker containerization
- [ ] CI/CD pipeline

---

## License

This is a demonstration project for educational purposes.

---

## Contact & Support

For questions about this project:
- Review the documentation files in `Documentation/` folder
- Check code comments for implementation details
- Examine test cases (when implemented)

---

**Created**: November 6, 2025  
**Framework**: .NET 9  
**Architecture**: Clean Layered Architecture with OOP Principles  
**Status**: Production-Ready Demo Project

---

## Quick Command Reference

```powershell
# Build project
dotnet build

# Run project
dotnet run

# Run with hot reload
dotnet watch run

# Restore packages
dotnet restore

# Clean build artifacts
dotnet clean

# Run tests (when implemented)
dotnet test

# Create new migration (if using real database)
dotnet ef migrations add MigrationName

# Update database (if using real database)
dotnet ef database update
```

---

**End of Project Guide** | See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for API details | See [OOP_DOCUMENTATION.md](OOP_DOCUMENTATION.md) for OOP concepts
