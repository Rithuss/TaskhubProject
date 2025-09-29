# TaskHubAPI
TaskHubAPI is a .NET 9 Web API project designed to provide task management capabilities. This project serves as the backend for managing tasks, users, and related operations.

## Features
- RESTful API endpoints for task management
- Built with ASP.NET Core (.NET 9)
- In-memory database for development and testing
- Swagger UI for API documentation and testing
- Easily extendable and maintainable architecture

## Project Structure
- `Models/` - Entity models (User, TaskItem)
- `DTOs/` - Data Transfer Objects for API requests/responses
- `Data/` - Database context (AppDbContext)
- `Program.cs` - Application entry point and configuration

## Technologies Used
- .NET 9
- ASP.NET Core Web API
- Entity Framework Core (InMemory)
- Swagger/OpenAPI

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Getting Started

1.**Navigate to the project directory:**cd TaskHubProject/TaskHubAPI 2. **Restore dependencies:**dotnet restore 3. **Build the project:**dotnet build 4. **Run the API:**dotnet run
## Usage
- The API will be available at `https://localhost:7043` or `http://localhost:5015` by default.
- Swagger UI is available at `/swagger` for interactive API documentation and testing.
- Use tools like Postman or curl to interact with the endpoints.

## Example API Endpoints
> **Note:** Actual controller endpoints may vary. Below are typical RESTful routes for task and user management.

### Tasks
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get a task by ID
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}` - Delete a task

### Users
- `GET /api/users` - Get all users
- `GET /api/users/{id}` - Get a user by ID
- `POST /api/users` - Create a new user
- `PUT /api/users/{id}` - Update a user
- `DELETE /api/users/{id}` - Delete a user

## Data Models
### User
- `Id` (int)
- `Name` (string)
- `Email` (string)

### TaskItem
- `Id` (int)
- `Title` (string)
- `Description` (string)
- `Status` (Pending, InProgress, Completed)
- `Priority` (Low, Medium, High)
- `DueDate` (DateTime)
- `UserId` (int)

## Running Tests
_No automated tests are included by default._

## Contact
For questions or support, please contact the project maintainer.

## Contributing
Contributions are welcome! Please open issues or submit pull requests for improvements.

## License
This project is licensed under the MIT License.
