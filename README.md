# Budgeting System - Microservices Architecture

A comprehensive budgeting system built with microservices architecture using Domain-Driven Design (DDD) principles and .NET 8.

## 🏗️ Architecture Overview

This system implements a microservices architecture with four core bounded contexts:

- **User Service** - User management and authentication
- **Budget Service** - Budget planning and allocation
- **Expense Service** - Expense tracking and management
- **Reporting Service** - Analytics and financial reporting
- **API Gateway** - Single entry point and request routing

Each service follows Clean Architecture principles with DDD patterns, ensuring scalability, maintainability, and clear domain boundaries.

## 🚀 Quick Start

### Prerequisites

- [Docker](https://www.docker.com/) and Docker Compose
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (for development)
- [Git](https://git-scm.com/)

### Running the System

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd BudgetingSystem
   git checkout microservices
   ```

2. **Start all services**
   ```bash
   cd microservices
   docker-compose up -d
   ```

3. **Verify services are running**
   ```bash
   docker-compose ps
   ```

4. **Access the API Gateway**
   - API Gateway: http://localhost:5001
   - Swagger Documentation: http://localhost:5001/swagger

## 📋 Services Overview

| Service | Purpose | Database | Port |
|---------|---------|----------|------|
| API Gateway | Request routing, authentication | - | 5001 |
| User Service | User management, authentication | UserDB | Internal |
| Budget Service | Budget planning, categories | BudgetDB | Internal |
| Expense Service | Expense tracking, categorization | ExpenseDB | Internal |
| Reporting Service | Analytics, reporting | ReportingDB | Internal |
| Redis | Message bus, caching | - | 6379 |

## 🏛️ Project Structure

```
microservices/
├── gateway/                     # API Gateway (Ocelot)
│   ├── Program.cs
│   ├── ocelot.json
│   └── Dockerfile
├── services/
│   ├── user-service/            # User Management Service
│   │   ├── API/                 # Controllers, middleware
│   │   ├── Application/         # Commands, queries, handlers
│   │   ├── Domain/              # Entities, value objects, events
│   │   ├── Infrastructure/      # Database, repositories
│   │   └── Dockerfile
│   ├── budget-service/          # Budget Planning Service
│   │   ├── API/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Dockerfile
│   ├── expense-service/         # Expense Tracking Service
│   │   ├── API/
│   │   ├── Application/
│   │   ├── Domain/
│   │   ├── Infrastructure/
│   │   └── Dockerfile
│   └── reporting-service/       # Reporting & Analytics Service
│       ├── API/
│       ├── Application/
│       ├── Domain/
│       ├── Infrastructure/
│       └── Dockerfile
└── docker-compose.yml          # Container orchestration
```

## 🔧 Development Setup

### Local Development

1. **Install dependencies**
   ```bash
   cd microservices/services/user-service
   dotnet restore
   ```

2. **Run individual service**
   ```bash
   dotnet run --project API
   ```

3. **Run database migrations**
   ```bash
   dotnet ef database update -p Infrastructure -s API
   ```

### Building Services

```bash
# Build all services
docker-compose build

# Build specific service
docker-compose build user-service
```

## 📡 API Documentation

### API Gateway Endpoints

All requests go through the API Gateway at `http://localhost:5001/api`

#### User Management
```http
POST   /api/users              # Create user
GET    /api/users/{id}         # Get user by ID
GET    /api/users/by-email/{email} # Get user by email
```

#### Budget Management
```http
POST   /api/budgets            # Create budget
GET    /api/budgets/{id}       # Get budget by ID
GET    /api/budgets/user/{userId} # Get user budgets
PUT    /api/budgets/{id}       # Update budget
```

#### Expense Management
```http
POST   /api/expenses           # Record expense
GET    /api/expenses/{id}      # Get expense by ID
GET    /api/expenses/user/{userId} # Get user expenses
PUT    /api/expenses/{id}      # Update expense
```

#### Reporting
```http
GET    /api/reports/user/{userId}/summary # Get spending summary
GET    /api/reports/user/{userId}/trends  # Get spending trends
```

### Example API Calls

**Create a new user:**
```bash
curl -X POST http://localhost:5001/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePassword123!"
  }'
```

**Create a budget:**
```bash
curl -X POST http://localhost:5001/api/budgets \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "user-guid-here",
    "name": "Monthly Budget",
    "totalAmount": 2000.00,
    "currency": "USD",
    "startDate": "2024-01-01",
    "endDate": "2024-01-31",
    "categories": [
      {
        "name": "Food",
        "allocatedAmount": 600.00
      },
      {
        "name": "Transportation",
        "allocatedAmount": 400.00
      }
    ]
  }'
```

## 🏗️ Architecture Patterns

### Domain-Driven Design (DDD)

Each service implements DDD with:
- **Entities**: Core business objects with identity
- **Value Objects**: Immutable objects representing concepts (Money, BudgetPeriod)
- **Aggregates**: Consistency boundaries (Budget aggregate contains BudgetCategories)
- **Domain Events**: Business events for inter-service communication
- **Repositories**: Data access abstractions

### CQRS (Command Query Responsibility Segregation)

- **Commands**: State-changing operations (CreateBudgetCommand)
- **Queries**: Data retrieval operations (GetBudgetQuery)
- **Handlers**: Process commands and queries separately

### Event-Driven Architecture

Services communicate via domain events published to Redis:
- `BudgetCreatedEvent` - When a new budget is created
- `ExpenseRecordedEvent` - When an expense is recorded
- `BudgetExceededEvent` - When spending exceeds budget limits

## 🛠️ Technology Stack

- **Runtime**: .NET 8.0
- **API Gateway**: Ocelot
- **Databases**: SQL Server 2022
- **Message Bus**: Redis
- **ORM**: Entity Framework Core
- **CQRS**: MediatR
- **Containerization**: Docker & Docker Compose
- **API Documentation**: Swagger/OpenAPI

## 🔍 Monitoring and Observability

### Health Checks
Each service exposes health check endpoints:
```
GET /health
```

### Logging
Structured logging is implemented using:
- ASP.NET Core built-in logging
- Console and file providers
- Structured JSON format

### Service Discovery
Services communicate via:
- Docker network resolution
- Environment variable configuration
- Health check validation

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests for specific service
cd services/budget-service
dotnet test
```

### Test Structure
- **Unit Tests**: Domain logic and business rules
- **Integration Tests**: Database and API endpoints
- **Contract Tests**: Inter-service communication

## 🚀 Deployment

### Production Deployment

1. **Build production images**
   ```bash
   docker-compose -f docker-compose.prod.yml build
   ```

2. **Deploy to container orchestrator**
   ```bash
   # Example for Docker Swarm
   docker stack deploy -c docker-compose.prod.yml budgeting-system
   ```

### Environment Configuration

Set environment variables for production:
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Production-DB-Connection"
export Redis__ConnectionString="Production-Redis-Connection"
```

## 📊 Performance Considerations

### Scaling Strategy
- **Horizontal Scaling**: Multiple service instances behind load balancer
- **Database Scaling**: Read replicas for query-heavy services
- **Caching**: Redis for frequently accessed data

### Optimization Tips
- Use async/await patterns throughout
- Implement connection pooling for databases
- Cache frequently accessed reference data
- Monitor and optimize database queries

## 🔐 Security

### Authentication & Authorization
- JWT tokens for authentication
- Role-based authorization
- API Gateway handles token validation

### Data Protection
- Sensitive data encryption at rest
- TLS/HTTPS for data in transit
- Input validation and sanitization

## 🐛 Troubleshooting

### Common Issues

**Services not starting:**
```bash
# Check logs
docker-compose logs [service-name]

# Restart specific service
docker-compose restart [service-name]
```

**Database connection issues:**
```bash
# Check database container status
docker-compose ps

# Reset database
docker-compose down -v
docker-compose up -d
```

**Port conflicts:**
```bash
# Check port usage
netstat -tulpn | grep :5001

# Modify ports in docker-compose.yml if needed
```

## 🤝 Contributing

### Development Workflow

1. **Create feature branch**
   ```bash
   git checkout -b feature/your-feature-name
   ```

2. **Make changes and test**
   ```bash
   dotnet test
   docker-compose up -d
   ```

3. **Commit and push**
   ```bash
   git add .
   git commit -m "feat: add new feature"
   git push origin feature/your-feature-name
   ```

4. **Create pull request**

### Code Standards
- Follow .NET coding conventions
- Write unit tests for new features
- Update documentation for API changes
- Use conventional commit messages

## 📚 Additional Resources

- [Architecture Documentation](ARCHITECTURE.md)
- [Domain-Driven Design Guide](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/)
- [.NET 8 Documentation](https://docs.microsoft.com/en-us/dotnet/)
- [Docker Compose Guide](https://docs.docker.com/compose/)

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🆘 Support

For questions and support:
- Create an issue in the repository
- Contact the development team
- Check the troubleshooting section above

---

**Happy coding! 🎉** 
