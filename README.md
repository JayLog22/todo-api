# 📝 Todo API

![Build Status](https://github.com/JayLog22/todo-api/actions/workflows/dotnet.yml/badge.svg)

A RESTful API for managing todo tasks built with .NET 8, following Clean Architecture principles.

## 🏗️ Architecture

This project implements **Clean Architecture** with the following layers:
```
TodoApi/
├── src/
│   ├── TodoApi.Core/          # Domain entities and interfaces
│   ├── TodoApi.Infrastructure/ # Data access and repositories
│   └── TodoApi/                # API controllers and services
└── tests/
    └── TodoApi.UnitTests/      # Unit tests
```

### Design Patterns
- **Repository Pattern** for data access abstraction
- **Dependency Injection** for loose coupling
- **DTO Pattern** for API contracts
- **Middleware Pattern** for global exception handling

---

## 🛠️ Technologies

- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8** - ORM for database access
- **SQL Server 2022** - Relational database
- **Docker & Docker Compose** - Containerization
- **FluentValidation** - Input validation
- **AutoMapper** - Object-to-object mapping
- **Swagger/OpenAPI** - API documentation
- **xUnit** - Unit testing framework
- **Moq** - Mocking framework
- **FluentAssertions** - Assertion library

---

## 🚀 Quick Start

### Prerequisites
- [Docker Desktop](https://www.docker.com/products/docker-desktop) installed

### Running with Docker (Recommended)

1. **Clone the repository**
```bash
   git clone https://github.com/JayLog22/todo-api.git
   cd todo-api
```

2. **Create environment file**
```bash
   cp .env.example .env
```

3. **Start the application**
```bash
   docker-compose up
```

4. **Open Swagger UI**
```
   http://localhost:5000/swagger
```

That's it! 🎉 The API and database are now running.

### Running locally (without Docker)

1. **Install prerequisites**
   - [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
   - [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads)

2. **Configure User Secrets**
   Store sensitive connection strings securely using .NET User Secrets instead of storing them in `appsettings.json`:

   ```bash
   # Navigate to the API project
   cd src/TodoApi

   # Initialize User Secrets for this project (creates a secrets ID in csproj)
   dotnet user-secrets init

   # Set the database connection string with your actual SQL Server password
   # Replace YOUR_PASSWORD with your actual SA password
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TodoApiDb;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"
   ```

   **How it works:**
   - User Secrets are stored locally in `%APPDATA%\Microsoft\UserSecrets\<guid>` (Windows) and **never committed to git**
   - In development, User Secrets override the values in `appsettings.json` automatically
   - The `appsettings.json` contains a placeholder that will be ignored during local development
   - In Docker, the connection string comes from environment variables in the `.env` file

   > **Why User Secrets?** Keeps sensitive data out of version control and prevents accidental exposure of credentials.

3. **Run migrations**
   ```bash
   dotnet ef database update --project src/TodoApi.Infrastructure --startup-project src/TodoApi
   ```

4. **Run the API**
   ```bash
   dotnet run --project src/TodoApi
   ```

---

## 📚 API Endpoints

### Tasks

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/todotasks` | Get all tasks (with optional filters) |
| `GET` | `/api/todotasks/{id}` | Get a specific task |
| `POST` | `/api/todotasks` | Create a new task |
| `PATCH` | `/api/todotasks/{id}` | Partially update a task |
| `PATCH` | `/api/todotasks/{id}/toggle` | Toggle task completion status |
| `DELETE` | `/api/todotasks/{id}` | Delete a task |

### Query Parameters (GET /api/todotasks)

- `isCompleted` - Filter by completion status (true/false)
- `priority` - Filter by priority (Low, Medium, High, Urgent)

### Example Requests

**Create a task:**
```json
POST /api/todotasks
{
  "title": "Complete project documentation",
  "description": "Write comprehensive README",
  "priority": "High",
  "dueDate": "2025-12-31",
  "tags": "documentation,urgent"
}
```

**Partial update:**
```json
PATCH /api/todotasks/{id}
{
  "description": "Updated description"
}
```

Only the provided fields will be updated.

---

## 🧪 Testing

The project includes comprehensive unit tests covering:
- Service layer business logic
- FluentValidation validators
- Edge cases and error scenarios

**Run tests:**
```bash
dotnet test
```

**Test coverage:**
- 16 Service tests
- 18 Validator tests
- Total: 34 tests

---

## 🏛️ Project Structure
```
TodoApi/
├── src/
│   ├── TodoApi/                    # API Layer
│   │   ├── Controllers/            # REST controllers
│   │   ├── Services/               # Business logic
│   │   │   ├── Interfaces/
│   │   │   └── Implementations/
│   │   ├── DTOs/                   # Data transfer objects
│   │   ├── Validators/             # FluentValidation rules
│   │   ├── Mappings/               # AutoMapper profiles
│   │   ├── Filters/                # Action filters
│   │   ├── Middleware/             # Exception handling
│   │   └── Converters/             # JSON converters
│   │
│   ├── TodoApi.Core/               # Domain Layer
│   │   ├── Entities/               # Domain models
│   │   └── Interfaces/
│   │       └── Repositories/       # Repository contracts
│   │
│   └── TodoApi.Infrastructure/     # Infrastructure Layer
│       ├── Data/                   # DbContext
│       ├── Configurations/         # EF configurations
│       ├── Repositories/           # Repository implementations
│       └── Migrations/             # EF migrations
│
├── tests/
│   └── TodoApi.UnitTests/          # Unit tests
│       ├── Services/
│       └── Validators/
│
├── Dockerfile                      # API container image
├── docker-compose.yml              # Multi-container setup
└── .env.example                    # Environment variables template
```

---

## 🔧 Configuration

### Environment Variables (Docker)

For Docker deployments, create a `.env` file in the project root:
```env
SA_PASSWORD=YourStrongPassword123!
DATABASE_NAME=TodoApiDb
```

The `docker-compose.yml` uses these variables to:
1. Set the SQL Server `sa` password
2. Build the connection string passed as an environment variable to the API container

**Docker Connection String Flow:**
```
.env file → docker-compose.yml environment variables → API container → Connection to DB
```

### User Secrets (Local Development)

For local development without Docker, use .NET User Secrets to securely store your actual connection string:

```bash
cd src/TodoApi

# One-time setup
dotnet user-secrets init

# Store your actual connection string with your SA password
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=TodoApiDb;User Id=sa;Password=YOUR_ACTUAL_PASSWORD;TrustServerCertificate=true;MultipleActiveResultSets=true"

# Verify it was set
dotnet user-secrets list
```

**Local Development Connection String Flow:**
```
SQL Server (local) ← User Secrets (password stored securely) ← dotnet run
                   ← appsettings.json (placeholder, ignored)
```

### Configuration Priority

When running the application:
1. **User Secrets** (local development) - highest priority, overrides everything
2. **Environment Variables** (Docker) - second priority
3. **appsettings.json** - fallback (placeholder values, should never be used in production)

---

## 🎯 Features

- ✅ **Clean Architecture** - Separation of concerns
- ✅ **Repository Pattern** - Data access abstraction
- ✅ **RESTful API** - Following REST best practices
- ✅ **Input Validation** - FluentValidation for request validation
- ✅ **Global Exception Handling** - Centralized error responses
- ✅ **Auto Mapping** - AutoMapper for DTO conversions
- ✅ **Flexible Date Parsing** - Multiple date formats supported
- ✅ **Enum as Strings** - User-friendly priority values
- ✅ **PATCH Support** - Partial updates of resources
- ✅ **Swagger Documentation** - Interactive API docs
- ✅ **Docker Support** - Easy deployment
- ✅ **Unit Tests** - Comprehensive test coverage
- ✅ **Database Seeding** - Sample data on startup

---

## 🐳 Docker Commands
```bash
# Start services
docker-compose up

# Start in background
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Remove everything (including database)
docker-compose down -v

# Rebuild after code changes
docker-compose up --build
```

---

## 📖 API Documentation

Once the application is running, visit:
- **Swagger UI:** http://localhost:5000/swagger
- **OpenAPI JSON:** http://localhost:5000/swagger/v1/swagger.json

---

## 🤝 Contributing

This is a portfolio project, but feedback and suggestions are welcome!

---

## 📄 License

This project is open source and available under the MIT License.

---

## 👤 Author

**Your Name**
- GitHub: [@JayLog22](https://github.com/JayLog22)
- LinkedIn: [Julián Vega](https://www.linkedin.com/in/julian-vega/)

---

## 🙏 Acknowledgments

Built as part of my journey learning Clean Architecture and modern .NET development practices.
