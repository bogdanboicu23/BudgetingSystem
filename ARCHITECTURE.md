# Microservices Architecture with Domain-Driven Design (DDD)

## Overview

This budgeting system implements a microservices architecture using Domain-Driven Design (DDD) principles. The system is designed around business domains with clear bounded contexts, ensuring scalability, maintainability, and domain alignment.

## Bounded Contexts

### 1. User Management Context (`user-service`)
- **Purpose**: Manages user registration, authentication, and profile management
- **Domain**: User identity and access management
- **Database**: UserDB
- **Key Aggregates**: User

### 2. Budget Planning Context (`budget-service`) ✅ **Fully Implemented**
- **Purpose**: Budget creation, planning, categorization, and tracking
- **Domain**: Budget planning and allocation
- **Database**: BudgetDB
- **Key Aggregates**: Budget (with BudgetCategories)

### 3. Expense Tracking Context (`expense-service`) 🚧 **Partially Implemented**
- **Purpose**: Expense recording, categorization, and management
- **Domain**: Financial transactions and expense tracking
- **Database**: ExpenseDB
- **Key Aggregates**: Expense

### 4. Financial Analytics Context (`reporting-service`)
- **Purpose**: Financial reporting, insights, and analytics
- **Domain**: Business intelligence and reporting
- **Database**: ReportingDB
- **Key Aggregates**: Report, Analytics

## DDD Architecture Structure

Each microservice follows the Clean Architecture principles with DDD layers:

### Domain Layer (`Domain/`)
The core business logic and domain models:

```
Domain/
├── Entities/           # Domain entities (Budget, Expense, etc.)
├── ValueObjects/       # Value objects (Money, BudgetId, etc.)
├── Aggregates/         # Aggregate roots and boundaries
├── DomainEvents/       # Domain events for integration
├── Services/           # Domain services
└── Repositories/       # Repository interfaces
```

**Key Components:**
- **Entities**: `Budget.cs`, `BudgetCategory.cs`
- **Value Objects**: `Money.cs`, `BudgetId.cs`, `UserId.cs`, `BudgetPeriod.cs`
- **Domain Events**: `BudgetCreatedEvent.cs`, `BudgetExceededEvent.cs`
- **Repository Interfaces**: `IBudgetRepository.cs`

### Application Layer (`Application/`)
Contains application logic and orchestrates domain operations:

```
Application/
├── Commands/           # Command objects (CQRS)
├── Queries/            # Query objects (CQRS)
├── DTOs/               # Data transfer objects
├── Handlers/           # Command/Query handlers
├── Services/           # Application services
└── Interfaces/         # Application interfaces
```

**Key Components:**
- **Commands**: `CreateBudgetCommand.cs`, `UpdateBudgetCommand.cs`
- **Queries**: `GetBudgetQuery.cs`, `GetUserBudgetsQuery.cs`
- **Handlers**: `CreateBudgetCommandHandler.cs`, `GetBudgetQueryHandler.cs`
- **DTOs**: `BudgetDto.cs`, `BudgetCategoryDto.cs`

### Infrastructure Layer (`Infrastructure/`)
Handles external concerns and persistence:

```
Infrastructure/
├── Data/               # Database context and configurations
├── Repositories/       # Repository implementations
├── ExternalServices/   # External API integrations
└── Messaging/          # Message bus implementations
```

**Key Components:**
- **DbContext**: `BudgetDbContext.cs`
- **Configurations**: `BudgetConfiguration.cs`, `BudgetCategoryConfiguration.cs`
- **Repositories**: `BudgetRepository.cs`

### API Layer (`API/`)
Handles HTTP concerns and external communication:

```
API/
├── Controllers/        # API controllers
├── Middleware/         # Custom middleware
└── Configuration/      # Service registration
```

**Key Components:**
- **Controllers**: `BudgetsController.cs`
- **Configuration**: `ServiceCollectionExtensions.cs`

## Design Patterns Used

### 1. CQRS (Command Query Responsibility Segregation)
- **Commands**: `CreateBudgetCommand`, `UpdateBudgetCommand`, `RecordExpenseCommand`
- **Queries**: `GetBudgetQuery`, `GetUserBudgetsQuery`, `GetActiveBudgetQuery`
- **Handlers**: Separate handlers for commands and queries using MediatR

### 2. Domain Events
- Events published when domain state changes
- Enables loose coupling between bounded contexts
- Examples: `BudgetCreatedEvent`, `BudgetExceededEvent`, `BudgetExpenseRecordedEvent`

### 3. Repository Pattern
- Abstracts data access logic
- Defined in Domain layer, implemented in Infrastructure layer
- Example: `IBudgetRepository` interface → `BudgetRepository` implementation

### 4. Aggregate Pattern
- `Budget` is an aggregate root that maintains consistency
- Contains `BudgetCategory` entities
- Enforces business rules and invariants

### 5. Value Objects
- Immutable objects representing domain concepts
- Examples: `Money`, `BudgetPeriod`, `BudgetId`
- Provide type safety and business logic encapsulation

## Key Features Implemented

### Budget Service Features:
1. **Budget Creation** with categories and period management
2. **Expense Tracking** within budget limits
3. **Budget Exceeded Alerts** via domain events
4. **Category Management** with allocation tracking
5. **Budget Queries** with filtering and user-specific access

### Technical Features:
1. **Clean Architecture** with dependency inversion
2. **Entity Framework Core** with code-first migrations
3. **MediatR** for CQRS implementation
4. **Domain Events** for inter-service communication
5. **RESTful APIs** with proper HTTP status codes
6. **Swagger Documentation** for API exploration

## Database Design

### Budget Service Database (BudgetDB)
```sql
Tables:
├── Budgets
│   ├── Id (uniqueidentifier, PK)
│   ├── UserId (uniqueidentifier)
│   ├── Name (nvarchar(200))
│   ├── TotalAmount (decimal(18,2))
│   ├── SpentAmount (decimal(18,2))
│   ├── Currency (nvarchar(3))
│   ├── Status (nvarchar(50))
│   ├── StartDate (datetime2)
│   ├── EndDate (datetime2)
│   ├── PeriodType (nvarchar(50))
│   ├── CreatedAt (datetime2)
│   └── LastModifiedAt (datetime2)
└── BudgetCategories
    ├── BudgetId (uniqueidentifier, PK)
    ├── Name (nvarchar(100), PK)
    ├── AllocatedAmount (decimal(18,2))
    ├── SpentAmount (decimal(18,2))
    └── Currency (nvarchar(3))
```

## Service Communication

### API Gateway Pattern
- **Gateway Service**: Routes requests to appropriate microservices
- **Load Balancing**: Distributes traffic across service instances
- **Authentication**: Centralized authentication and authorization
- **API Aggregation**: Combines responses from multiple services

### Message Bus (Redis)
- **Event Publishing**: Services publish domain events
- **Event Subscription**: Services subscribe to relevant events
- **Async Communication**: Loose coupling between services
- **Event Sourcing**: Potential for event replay and audit trails

## Docker Configuration

Each service is containerized with:
- **Multi-stage builds** for optimized images
- **Health checks** for container monitoring
- **Environment-specific configurations**
- **Database migrations** on startup

## Running the System

### Prerequisites:
- Docker and Docker Compose
- .NET 8.0 SDK (for development)

### Startup:
```bash
cd microservices
docker-compose up -d
```

### Services:
- **API Gateway**: http://localhost:5000
- **Budget Service**: Internal (via gateway)
- **User Service**: Internal (via gateway)
- **Expense Service**: Internal (via gateway)
- **Reporting Service**: Internal (via gateway)

## API Endpoints

### Budget Service (via Gateway):
```
GET    /api/budgets/{id}              # Get budget by ID
GET    /api/budgets/user/{userId}     # Get user budgets
GET    /api/budgets/user/{userId}/active # Get active budget
POST   /api/budgets                   # Create budget
PUT    /api/budgets/{id}              # Update budget
POST   /api/budgets/{id}/expenses     # Record expense
```

## Development Guidelines

### Domain Layer Rules:
1. No dependencies on external frameworks
2. Rich domain models with business logic
3. Domain events for important state changes
4. Aggregate roots maintain consistency boundaries

### Application Layer Rules:
1. Orchestrates domain operations
2. Handles cross-cutting concerns
3. Implements use cases via commands/queries
4. Maps between domain and DTOs

### Infrastructure Layer Rules:
1. Implements domain interfaces
2. Handles persistence and external services
3. Contains framework-specific code
4. No business logic

### API Layer Rules:
1. Thin controllers with minimal logic
2. Proper HTTP status codes
3. Input validation and error handling
4. API documentation

## Future Enhancements

1. **Event Sourcing**: Store domain events as source of truth
2. **CQRS Read Models**: Separate read models for queries
3. **Saga Pattern**: Manage distributed transactions
4. **API Versioning**: Support multiple API versions
5. **Monitoring**: Add logging, metrics, and tracing
6. **Security**: Implement JWT authentication and authorization
7. **Testing**: Add comprehensive unit and integration tests

## Conclusion

This microservices architecture with DDD provides:
- **Scalability**: Independent service scaling
- **Maintainability**: Clear separation of concerns
- **Domain Alignment**: Business-focused design
- **Flexibility**: Easy to extend and modify
- **Resilience**: Fault isolation between services

The implementation demonstrates modern .NET architecture patterns and provides a solid foundation for a production budgeting system.