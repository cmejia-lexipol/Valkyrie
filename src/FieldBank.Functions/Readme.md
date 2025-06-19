# FieldBank Lambda Functions

This project contains AWS Lambda functions for managing Field entities using Clean Architecture principles.

## Project Structure

```
└─ FieldBank.Functions
      ├─ FunctionsStartup.cs        ← Generic Host startup, DI, EF Core (UseNpgsql), Serilog  
      ├─ TestFunctions.cs           ← Unified testing function for all CRUD operations
      ├─ Function.cs                ← Original function for production use
      ├─ aws-lambda-tools-defaults.json ← AWS Lambda Test Tool configuration
      └─ Handlers/                  ← Individual Lambda functions for deployment
          ├─ GetFieldsFunction.cs
          ├─ GetFieldByIdFunction.cs
          ├─ CreateFieldFunction.cs
          ├─ UpdateFieldFunction.cs
          ├─ DeleteFieldFunction.cs
```

## Individual Lambda Functions

### Function Details

Each CRUD operation has its own dedicated Lambda function with specific responsibilities:

#### 1. GetFieldsFunction
**Purpose**: Retrieve all fields
**Input**: No parameters required
**Output**: JSON list of all fields
**Handler**: `GetFieldsFunction::FunctionHandler`

#### 2. GetFieldByIdFunction
**Purpose**: Retrieve a specific field by ID
**Input**: Field ID
**Output**: JSON of the found field or error message
**Handler**: `GetFieldByIdFunction::FunctionHandler`

#### 3. CreateFieldFunction
**Purpose**: Create a new field
**Input**: Field data to create
**Output**: JSON of the created field
**Handler**: `CreateFieldFunction::FunctionHandler`

#### 4. UpdateFieldFunction
**Purpose**: Update an existing field
**Input**: ID and new field data
**Output**: JSON of the updated field
**Handler**: `UpdateFieldFunction::FunctionHandler`

#### 5. DeleteFieldFunction
**Purpose**: Delete a field by ID
**Input**: Field ID to delete
**Output**: Confirmation message
**Handler**: `DeleteFieldFunction::FunctionHandler`

## Function Configuration

### Constructor Pattern

Each function implements a dual constructor pattern:

```csharp
// Constructor for dependency injection (production use)
public FunctionName(IFieldService fieldService)
{
    _fieldService = fieldService;
}

// Parameterless constructor for Lambda test tool
public FunctionName()
{
    // Configure DI container for testing
    var services = new ServiceCollection();
    // ... DI configuration
}
```

### Error Handling

All functions include comprehensive error handling:
- **ArgumentException**: Validation errors
- **InvalidOperationException**: Business logic errors
- **Exception**: General errors
- **Structured Logging**: Using Lambda context

## Deployment Configuration

### AWS Lambda Test Tool

For local testing, the project is configured to use `TestFunctions`:

```json
{
  "function-handler": "FieldBank.Functions::FieldBank.Functions.TestFunctions::FunctionHandler",
  "function-memory-size": 256,
  "function-timeout": 30,
  "function-runtime": "dotnet8"
}
```

### Production Deployment

Each function can be deployed independently:

```bash
# Deploy individual functions
dotnet lambda deploy-function GetFieldsFunction
dotnet lambda deploy-function CreateFieldFunction
dotnet lambda deploy-function GetFieldByIdFunction
dotnet lambda deploy-function UpdateFieldFunction
dotnet lambda deploy-function DeleteFieldFunction
```

## Function-Specific Testing

### Individual Function Testing

Each function can be tested independently using the AWS Lambda Test Tool with specific input formats.

### Integration Testing

Functions can be tested together to verify end-to-end workflows.

### Production Testing

Deploy functions individually and test through API Gateway endpoints.

## Performance Considerations

### Cold Start Optimization

- **Connection Pooling**: EF Core connection pooling
- **Memory Configuration**: 256MB minimum for optimal performance
- **Timeout Settings**: 30 seconds for database operations

### Resource Management

- **Disposable Resources**: Proper disposal of DbContext
- **Memory Management**: Efficient object creation and disposal
- **Async Operations**: Full async/await pattern usage

## Security Best Practices

### Input Validation

- **Field-level validation**: Validate all inputs at function level
- **SQL Injection Prevention**: Use parameterized queries via EF Core
- **Error Information**: Don't expose sensitive information in errors

### Access Control

- **Function-level permissions**: Granular IAM roles per function
- **Resource isolation**: Each function has minimal required permissions
- **Audit logging**: Track all operations for security monitoring

## Monitoring and Observability

### CloudWatch Integration

- **Function Metrics**: Duration, errors, throttles
- **Custom Metrics**: Business-specific metrics per function
- **Log Groups**: Separate log groups for each function

### X-Ray Tracing

- **Distributed Tracing**: Track requests across functions
- **Performance Analysis**: Identify bottlenecks per function
- **Error Tracking**: Trace errors to specific function calls

## Function-Specific Configuration

### Environment Variables

Each function can have specific environment variables:

```bash
# Database configuration
ConnectionStrings__DefaultConnection=your-connection-string

# Logging configuration
LOG_LEVEL=Information
AWS_REGION=us-east-1

# Function-specific settings
FIELD_CACHE_TTL=300
MAX_FIELDS_PER_REQUEST=100
```

### Memory and Timeout Settings

```json
{
  "GetFieldsFunction": {
    "memory-size": 256,
    "timeout": 30
  },
  "CreateFieldFunction": {
    "memory-size": 512,
    "timeout": 60
  },
  "UpdateFieldFunction": {
    "memory-size": 512,
    "timeout": 60
  }
}
```

## API Gateway Integration

### REST API Endpoints

When integrated with API Gateway, functions map to REST endpoints:

```
GET    /fields              → GetFieldsFunction
GET    /fields/{id}         → GetFieldByIdFunction
POST   /fields              → CreateFieldFunction
PUT    /fields/{id}         → UpdateFieldFunction
DELETE /fields/{id}         → DeleteFieldFunction
```

### Request/Response Mapping

Each function handles specific request/response formats:

```json
// GetFieldByIdFunction Input
{
  "pathParameters": {
    "id": "1"
  }
}

// CreateFieldFunction Input
{
  "body": {
    "name": "email",
    "label": "Email Address",
    "description": "User's email address"
  }
}
```

## Troubleshooting Functions

### Common Function Issues

1. **Memory Issues**: Increase function memory allocation
2. **Timeout Issues**: Optimize database queries or increase timeout
3. **Connection Issues**: Check database connectivity and connection pooling
4. **Permission Issues**: Verify IAM roles and policies

### Debugging Strategies

1. **CloudWatch Logs**: Review function execution logs
2. **X-Ray Traces**: Analyze request flow and performance
3. **Local Testing**: Use AWS Lambda Test Tool for debugging
4. **Error Monitoring**: Set up CloudWatch alarms for errors

## Next Steps

1. **API Gateway Integration**: Configure REST API endpoints
2. **Authentication**: Implement JWT-based authentication
3. **Rate Limiting**: Configure API Gateway throttling
4. **Caching**: Implement response caching
5. **Monitoring**: Set up comprehensive monitoring and alerting
6. **Testing**: Add comprehensive unit and integration tests
