# FieldBank - AWS Lambda Clean Architecture Project

A .NET 8 AWS Lambda project implementing Clean Architecture principles for managing Field entities with PostgreSQL database.

## Project Overview

FieldBank is a serverless application built with AWS Lambda that provides CRUD operations for Field entities. The project follows Clean Architecture principles to ensure maintainability, testability, and scalability.

## Architecture

```
└─ FieldBank/
    ├─ src/
    │   ├─ FieldBank.Domain/           ← Entities, Interfaces, Business Rules
    │   ├─ FieldBank.Application/      ← Business Logic, Services
    │   ├─ FieldBank.Infrastructure/   ← Data Access, External Services
    │   └─ FieldBank.Functions/        ← AWS Lambda Handlers, API Endpoints
    └─ tests/                          ← Unit and Integration Tests
```

### Clean Architecture Layers

- **Domain Layer**: Contains entities, interfaces, and business rules
- **Application Layer**: Contains business logic, services, and use cases
- **Infrastructure Layer**: Handles data persistence, external services, and configurations
- **Functions Layer**: AWS Lambda handlers and API endpoints

## Features

- ✅ **CRUD Operations**: Complete Create, Read, Update, Delete functionality
- ✅ **Clean Architecture**: Proper separation of concerns
- ✅ **Entity Framework Core**: PostgreSQL database with migrations
- ✅ **Dependency Injection**: Proper DI container configuration
- ✅ **Structured Logging**: Serilog integration for observability
- ✅ **AWS Lambda**: Serverless deployment ready
- ✅ **Local Testing**: AWS Lambda Test Tool integration
- ✅ **Audit Fields**: Automatic tracking of Created/Modified dates and users

## Quick Start

### Prerequisites

1. **.NET 8.0 SDK**
2. **PostgreSQL** (running on localhost:5432)
3. **AWS CLI** (for deployment)
4. **AWS Lambda Test Tool** (for local testing)

### Setup

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd FieldBank
   ```

2. **Build the project**:
   ```bash
   dotnet build
   ```

3. **Run database migrations**:
   ```bash
   dotnet ef database update --project src/FieldBank.Infrastructure --startup-project src/FieldBank.Functions
   ```

4. **Test locally**:
   - Open `src/FieldBank.Functions/TestFunctions.cs` in VS Code/Cursor
   - Press F5 and select "AWS Lambda Test Tool"
   - Use the examples from the testing section below

## Available Functions

### Individual Lambda Functions

Each CRUD operation has its own dedicated Lambda function:

- **GetFieldsFunction**: Retrieve all fields
- **GetFieldByIdFunction**: Retrieve field by ID
- **CreateFieldFunction**: Create new field
- **UpdateFieldFunction**: Update existing field
- **DeleteFieldFunction**: Delete field by ID

### Unified Testing Function

For local testing, use the `TestFunctions` class that handles all operations through a single endpoint.

## Testing

### Local Testing Setup

1. **Build the project first**:
   ```bash
   dotnet build src/FieldBank.Functions --configuration Debug
   ```

2. **Verify PostgreSQL is running** on localhost:5432

3. **Ensure the `FieldBank` database exists**

### Opening AWS Lambda Test Tool

#### In VS Code/Cursor:
1. Open VS Code/Cursor
2. Navigate to `src/FieldBank.Functions/TestFunctions.cs`
3. Press F5 or use "Run and Debug" command
4. Select "AWS Lambda Test Tool"

#### In Visual Studio:
1. Open Visual Studio
2. Right-click on the `FieldBank.Functions` project
3. Select "Debug" → "Start New Instance"
4. Select "AWS Lambda Test Tool"

### Test Examples

#### 🔍 GET ALL - Retrieve all fields
```json
{
  "Operation": "getall"
}
```

#### 🔍 GET BY ID - Retrieve field by ID
```json
{
  "Operation": "getbyid",
  "Id": 1
}
```

#### ➕ CREATE - Create new field
```json
{
  "Operation": "create",
  "Name": "email",
  "Label": "Email Address",
  "Description": "User's email address"
}
```

#### ✏️ UPDATE - Update existing field
```json
{
  "Operation": "update",
  "Id": 1,
  "Name": "email_updated",
  "Label": "Email Address Updated",
  "Description": "Updated email address field"
}
```

#### 🗑️ DELETE - Delete field
```json
{
  "Operation": "delete",
  "Id": 1
}
```


## Troubleshooting

### Common Issues

#### ❌ Error: "Failed to find type FieldBank.Functions.TestFunctions"

**Cause**: The AWS Lambda Test Tool cannot find the compiled class.

**Solution**:
1. Build the project first: `dotnet build src/FieldBank.Functions --configuration Debug`
2. Verify the DLL file exists: `src/FieldBank.Functions/bin/Debug/net8.0/FieldBank.Functions.dll`
3. Restart VS Code/Cursor after building

#### ❌ Error: "ConnectionString property has not been initialized"

**Cause**: Database connection issues.

**Solution**:
- Ensure PostgreSQL is running on localhost:5432
- Verify the `FieldBank` database exists
- Confirm the `postgres` user with password `password` has access

#### ❌ Error: "No parameterless constructor"

**Cause**: Wrong function class being used.

**Solution**:
- Ensure you're using the `TestFunctions` class (not `Function`)
- Verify the `function-handler` in `aws-lambda-tools-defaults.json` points to `TestFunctions`

#### ❌ Error: "Invalid operation"

**Cause**: Incorrect operation name.

**Solution**:
- Verify the `Operation` field is spelled correctly: `getall`, `getbyid`, `create`, `update`, `delete`
- Ensure the JSON is properly formatted

## Deployment

### Using AWS CLI

1. **Install AWS Lambda Tools**:
   ```bash
   dotnet tool install -g Amazon.Lambda.Tools
   ```

2. **Deploy individual functions**:
   ```bash
   cd src/FieldBank.Functions
   dotnet lambda deploy-function GetFieldsFunction
   dotnet lambda deploy-function CreateFieldFunction
   # ... deploy other functions
   ```

### Using Visual Studio

1. Right-click on the `FieldBank.Functions` project
2. Select "Publish to AWS Lambda"
3. Configure function settings and deploy

## Configuration

### Database Connection

The application uses PostgreSQL with the following default configuration:
- **Host**: localhost
- **Port**: 5432
- **Database**: FieldBank
- **Username**: postgres
- **Password**: password

### Environment Variables

For production deployment, configure these environment variables:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `AWS_REGION`: AWS region for Lambda deployment
- `LOG_LEVEL`: Logging level (Debug, Information, Warning, Error)

## Project Structure

### Domain Layer (`FieldBank.Domain`)

- **Entities**: Business objects (Field, BaseEntity)
- **Interfaces**: Repository contracts (IFieldRepository)

### Application Layer (`FieldBank.Application`)

- **Services**: Business logic services (IFieldService, FieldService)
- **Extensions**: DI registration extensions

### Infrastructure Layer (`FieldBank.Infrastructure`)

- **Persistence**: Entity Framework configuration and DbContext
- **Repositories**: Data access implementations
- **Caching**: Redis cache service (if needed)
- **Migrations**: Database schema migrations

### Functions Layer (`FieldBank.Functions`)

- **Handlers**: Individual Lambda function handlers
- **Startup**: Generic Host configuration
- **Testing**: Test functions and configurations

## Best Practices

### Code Organization

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Dependency Inversion**: High-level modules don't depend on low-level modules
3. **Interface Segregation**: Clients depend only on interfaces they use
4. **Single Responsibility**: Each class has one reason to change

### Testing Strategy

1. **Unit Tests**: Test individual components in isolation
2. **Integration Tests**: Test component interactions
3. **End-to-End Tests**: Test complete workflows
4. **Performance Tests**: Test under load conditions

### Security

1. **Input Validation**: Validate all inputs at function level
2. **Error Handling**: Don't expose sensitive information in errors
3. **Authentication**: Implement proper authentication (future enhancement)
4. **Authorization**: Implement role-based access control (future enhancement)

## Monitoring and Observability

### CloudWatch Integration

- **Logs**: Structured logging with Serilog
- **Metrics**: Custom metrics for business operations
- **Alarms**: Set up alarms for error rates and performance
- **Dashboards**: Create dashboards for monitoring

### X-Ray Tracing

- **Distributed Tracing**: Track requests across services
- **Performance Analysis**: Identify bottlenecks
- **Error Tracking**: Trace errors to their source

## Contributing

1. Follow Clean Architecture principles
2. Write comprehensive tests
3. Update documentation
4. Follow the established naming conventions
5. Ensure all builds pass

## Next Steps

1. **Authentication & Authorization**: Implement JWT-based authentication
2. **API Gateway**: Configure REST API endpoints
3. **Caching**: Implement Redis caching for performance
4. **Monitoring**: Set up comprehensive monitoring and alerting
5. **CI/CD**: Implement automated deployment pipeline
6. **Testing**: Add comprehensive unit and integration tests

## License

This project is licensed under the MIT License - see the LICENSE file for details. 