# Valkyrie - AWS Lambda Clean Architecture Project

A .NET 8 AWS Lambda project implementing Clean Architecture principles for managing Field entities with PostgreSQL database.

---

## Database Migrations

To set up or update your local PostgreSQL database schema, run the following command:

```bash
dotnet ef database update --project src/Valkyrie.Infrastructure --startup-project src/Valkyrie.Functions
```

- This will apply the latest migrations to the database specified in your configuration.
- Make sure your PostgreSQL server is running and accessible with the credentials in your `appsettings.json` or environment variables.

---

## Local Testing: Configuration Notes

- **appsettings files**
  - For local testing with the Functions solution, ensure that your `appsettings.json` (and optionally `appsettings.Development.json`) in `src/Valkyrie.Functions` are up to date with the correct connection strings and settings for your local environment.
  - The Functions solution will use these files when running locally, so any changes to database, logging, or other settings should be reflected here.

---

## Why are there two solutions? (Valkyrie.Api and Valkyrie.Functions)

### Valkyrie.Api
- **Traditional ASP.NET Core Minimal API**
- Exposes HTTP endpoints directly, ideal for local development and quick testing.
- Easily test the API using Swagger.
- **Typical deployment:** Container, EC2, ECS, App Service, or Lambda with an adapter.

### Valkyrie.Functions
- **Serverless solution for AWS Lambda**
- Each operation (get, create, update, delete, etc.) is an independent Lambda function with its own handler.
- **Typical deployment:** AWS Lambda, integrated with API Gateway.

---

## How to test each solution?

### Valkyrie.Api (Traditional API with Swagger)

- **Purpose:**  
  Expose and test the API in a traditional way, with REST endpoints and interactive documentation.

- **How to start:**
  - **Visual Studio:**
    1. Right-click the `Valkyrie.Api` project
    2. Select "Set as Startup Project"
    3. Press F5 or "Start Debugging"
  - **VS Code:**
    1. Open the project folder
    2. Run:
       ```bash
       dotnet run --project src/Valkyrie.Api
       ```
    3. Open your browser at [http://localhost:5024/swagger](http://localhost:5024/swagger) (or the port shown in the console)

- **How to test:**  
  Use the Swagger UI to interactively test all endpoints (`/fields`, `/fields/{id}`, `/categories`, `/fieldtypes`, etc.).

---

### Valkyrie.Functions (AWS Lambda)

- **Purpose:**  
  Test and deploy your logic as individual Lambda functions, each handling a specific operation.

- **Key structure:**
  - **Handlers/**: Each file is a Lambda for a specific operation (e.g., `GetFieldsFunction`, `CreateFieldFunction`, `GetCategoriesFunction`, etc.)
  - **serverless.template**: Defines all Lambda functions for AWS Lambda Test Tool to automatically detect them

- **How to test:**
  - **Visual Studio:**
    1. Right-click the `Valkyrie.Functions` project
    2. Select "Debug" → "Start New Instance"
    3. Choose "AWS Lambda Test Tool" as the launch profile
    4. Use the Test Tool panel to select and test individual handlers
  - **VS Code:**
    1. Open the project folder
    2. Install the AWS Lambda Test Tool if you don't have it:
       ```bash
       dotnet tool install -g Amazon.Lambda.TestTool-8.0
       ```
    3. Run:
       ```bash
       dotnet lambda test-tool-8.0
       ```
    4. Open your browser at the URL shown in the console (usually [http://localhost:5050](http://localhost:5050))
    5. Select any individual handler from the dropdown to test specific operations

---

## Quick Start (Summary of how to run and test)

### Valkyrie.Api (Swagger)
- **Visual Studio:** F5 on `Valkyrie.Api`
- **VS Code:** `dotnet run --project src/Valkyrie.Api`
- **Go to:** `/swagger`

### Valkyrie.Functions (Lambda)
- **Visual Studio:** F5 on `Valkyrie.Functions` with the "AWS Lambda Test Tool" profile
- **VS Code:**
  - Install the test tool
  - `dotnet lambda test-tool-8.0`
  - Use your browser to test individual handlers

---

## Project Overview

Valkyrie is a serverless application built with AWS Lambda that provides CRUD operations for Field entities with related Categories and FieldTypes. The project follows Clean Architecture principles to ensure maintainability, testability, and scalability.

## Architecture

```
└─ Valkyrie/
    ├─ src/
    │   ├─ Valkyrie.Domain/           ← Entities, Interfaces, Business Rules
    │   ├─ Valkyrie.Application/      ← Business Logic, Services
    │   ├─ Valkyrie.Infrastructure/   ← Data Access, External Services
    │   ├─ Valkyrie.Functions/        ← AWS Lambda Handlers
    │   └─ Valkyrie.Api/              ← API Endpoints
    └─ tests/                          ← Unit and Integration Tests
```

### Clean Architecture Layers

- **Domain Layer**: Contains entities, interfaces, and business rules
- **Application Layer**: Contains business logic, services, and use cases
- **Infrastructure Layer**: Handles data persistence, external services, and configurations
- **Functions Layer**: AWS Lambda handlers and API endpoints
- **API Layer**: API endpoints

## Features

- ✅ **CRUD Operations**: Complete Create, Read, Update, Delete functionality for Fields
- ✅ **Category Management**: Full CRUD operations for Categories
- ✅ **FieldType Management**: Read operations for FieldTypes with enum support
- ✅ **Clean Architecture**: Proper separation of concerns
- ✅ **Entity Framework Core**: PostgreSQL database with migrations
- ✅ **Dependency Injection**: Proper DI container configuration
- ✅ **Structured Logging**: Serilog integration for observability
- ✅ **AWS Lambda**: Serverless deployment ready
- ✅ **Local Testing**: AWS Lambda Test Tool integration
- ✅ **Audit Fields**: Automatic tracking of Created/Modified dates and users
- ✅ **Relationships**: Fields can be associated with Categories and FieldTypes

## Available Functions

### Individual Lambda Functions

Each CRUD operation has its own dedicated Lambda function:

#### Field Operations
- **GetFieldsFunction**: Retrieve all fields with related Category data
- **GetFieldByIdFunction**: Retrieve field by ID with related Category data
- **CreateFieldFunction**: Create new field with CategoryId
- **UpdateFieldFunction**: Update existing field with CategoryId
- **DeleteFieldFunction**: Delete field by ID

#### Category Operations
- **GetCategoriesFunction**: Retrieve all categories
- **CreateCategoryFunction**: Create new category
- **UpdateCategoryFunction**: Update existing category
- **DeleteCategoryFunction**: Delete category by ID

#### FieldType Operations
- **GetFieldTypesFunction**: Retrieve all field types (Date, Time, Number, Text, Boolean)

## Testing

### Local Testing Setup

1. **Build the project first**:
   ```bash
   dotnet build src/Valkyrie.Functions --configuration Debug
   ```

2. **Verify PostgreSQL is running** on localhost:5432

3. **Ensure the `Valkyrie` database exists**

### Opening AWS Lambda Test Tool

#### In VS Code/Cursor:
1. Open VS Code/Cursor
2. Navigate to `src/Valkyrie.Functions/`
3. Press F5 or use "Run and Debug" command
4. Select "AWS Lambda Test Tool"

#### In Visual Studio:
1. Open Visual Studio
2. Right-click on the `Valkyrie.Functions` project
3. Select "Debug" → "Start New Instance"
4. Select "AWS Lambda Test Tool"

## Troubleshooting

### Common Issues

#### ❌ Error: "Failed to find type [HandlerName]"

**Cause**: The AWS Lambda Test Tool cannot find the compiled class.

**Solution**:
1. Build the project first: `dotnet build src/Valkyrie.Functions --configuration Debug`
2. Verify the DLL file exists: `src/Valkyrie.Functions/bin/Debug/net8.0/Valkyrie.Functions.dll`
3. Restart VS Code/Cursor after building

#### ❌ Error: "ConnectionString property has not been initialized"

**Cause**: Database connection issues.

**Solution**:
- Ensure PostgreSQL is running on localhost:5432
- Verify the `Valkyrie` database exists
- Confirm the `postgres` user with password `password` has access

#### ❌ Error: "No parameterless constructor"

**Cause**: Wrong function class being used.

**Solution**:
- Ensure you're using the correct handler class from the dropdown
- Verify the `serverless.template` file contains all your handlers

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
   cd src/Valkyrie.Functions
   dotnet lambda deploy-function GetFieldsFunction
   dotnet lambda deploy-function CreateFieldFunction
   dotnet lambda deploy-function GetCategoriesFunction
   # ... deploy other functions
   ```

### Using Visual Studio

1. Right-click on the `Valkyrie.Functions` project
2. Select "Publish to AWS Lambda"
3. Configure function settings and deploy

## Configuration

### Database Connection

The application uses PostgreSQL with the following default configuration:
- **Host**: localhost
- **Port**: 5432
- **Database**: Valkyrie
- **Username**: postgres
- **Password**: password

### Environment Variables

For production deployment, configure these environment variables:
- `ConnectionStrings__DefaultConnection`: Database connection string
- `AWS_REGION`: AWS region for Lambda deployment
- `LOG_LEVEL`: Logging level (Debug, Information, Warning, Error)

## Project Structure

### Domain Layer (`Valkyrie.Domain`)

- **Entities**: Business objects (Field, Category, FieldType, BaseEntity)
- **Interfaces**: Repository contracts (IFieldRepository, ICategoryRepository, IFieldTypeRepository)
- **Enums**: FieldTypeEnum (Date, Time, Number, Text, Boolean)

### Application Layer (`Valkyrie.Application`)

- **Features**: CQRS pattern with Commands and Queries
  - **Fields**: CRUD operations for Field entities
  - **Categories**: CRUD operations for Category entities
  - **FieldTypes**: Query operations for FieldType entities
- **Common**: DTOs, AutoMapper profiles, and service extensions
- **Extensions**: DI registration extensions

### Infrastructure Layer (`Valkyrie.Infrastructure`)

- **Persistence**: Entity Framework configuration and DbContext
- **Repositories**: Data access implementations for all entities
- **Caching**: Redis cache service (if needed)
- **Migrations**: Database schema migrations
- **SeedData**: Initial data seeding for Categories and FieldTypes

### Functions Layer (`Valkyrie.Functions`)

- **Handlers**: Individual Lambda function handlers for each operation
- **FunctionsStartup**: Generic Host configuration and dependency injection
- **serverless.template**: AWS Lambda function definitions for testing

## Best Practices

### Code Organization

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Dependency Inversion**: High-level modules don't depend on low-level modules
3. **Interface Segregation**: Clients depend only on interfaces they use
4. **Single Responsibility**: Each class has one reason to change
5. **CQRS Pattern**: Separate commands and queries for better maintainability

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
7. **Validation**: Add FluentValidation for input validation
8. **Rate Limiting**: Implement API rate limiting
