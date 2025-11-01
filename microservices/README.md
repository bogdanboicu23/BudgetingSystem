# Budgeting System - Microservices

This is the microservices implementation of the budgeting system, showcasing how to decompose the system into independent, deployable services.

## Architecture Overview

The system is decomposed into the following services:

### Core Services
- **User Service**: Handles user management, authentication, and authorization
- **Budget Service**: Manages budget creation, updates, and budget-related operations
- **Expense Service**: Handles expense recording, categorization, and tracking
- **Reporting Service**: Provides analytics, reports, and data aggregation

### Infrastructure Services
- **API Gateway**: Single entry point using Ocelot for routing and load balancing
- **Message Bus**: Redis for inter-service communication and event publishing
- **Databases**: Separate SQL Server instances for each service ensuring data isolation

## Key Characteristics of Microservices

✅ **Independent Deployment**: Each service can be deployed independently
✅ **Database per Service**: Each service owns its data and database
✅ **Service Communication**: HTTP APIs and async messaging via Redis
✅ **Technology Flexibility**: Each service can use different tech stacks
✅ **Independent Scaling**: Scale services based on individual load patterns

## Running the Application

### Prerequisites
- Docker and Docker Compose
- .NET 8 SDK (for local development)

### Using Docker Compose
```bash
docker-compose up --build
```

The API Gateway will be available at `http://localhost:5000`

### Individual Service Development
Each service can be run independently:

```bash
cd services/user-service
dotnet run
```

## Service Communication

### Synchronous Communication
- API Gateway routes requests to appropriate services
- Services make HTTP calls to other services when needed
- Circuit breaker patterns implemented for resilience

### Asynchronous Communication
- Event-driven communication via Redis
- Domain events published when state changes occur
- Event handlers in subscribing services

## API Routes (via Gateway)

- **Users**: `GET/POST/PUT/DELETE /api/users/*`
- **Budgets**: `GET/POST/PUT/DELETE /api/budgets/*`
- **Expenses**: `GET/POST/PUT/DELETE /api/expenses/*`
- **Reports**: `GET/POST /api/reports/*`

## Service Dependencies

```
API Gateway
├── User Service → User DB
├── Budget Service → Budget DB + User Service
├── Expense Service → Expense DB + User Service + Budget Service
└── Reporting Service → Reporting DB + All Services

Message Bus (Redis) ← All Services
```

## Benefits vs. Modular Monolith

**Pros:**
- Independent scaling and deployment
- Technology diversity (different languages/frameworks per service)
- Team autonomy and ownership
- Fault isolation (one service failure doesn't crash others)
- Better suited for large, distributed teams

**Cons:**
- Higher operational complexity
- Network latency between services
- Distributed transaction challenges
- More complex testing (integration testing across services)
- Increased infrastructure requirements

## Deployment Strategy

Each service is containerized and can be deployed using:
- Docker Compose (development)
- Kubernetes (production)
- Cloud container services (Azure Container Apps, AWS ECS, etc.)

## Monitoring and Observability

- Health checks for each service
- Distributed tracing capabilities
- Centralized logging
- Metrics collection for each service