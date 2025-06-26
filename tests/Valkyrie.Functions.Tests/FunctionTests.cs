using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using Valkyrie.Application.Features.Fields.Commands.CreateField;
using Valkyrie.Application.Features.Fields.Commands.UpdateField;
using Valkyrie.Application.Features.Fields.Commands.DeleteField;
using Valkyrie.Application.Features.Fields.Queries.GetAllFields;
using Valkyrie.Application.Features.Fields.Queries.GetFieldById;
using Valkyrie.Application.Common.DTOs;
using Assert = Xunit.Assert;

namespace Valkyrie.Functions.Tests;
public class FunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly Function _function;

    public FunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();

        // Setup the Logger to avoid NullReferenceException
        var mockLambdaLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLambdaLogger.Object);

        _function = new Function(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_GetAll_ReturnsFieldsAsJson()
    {
        // Arrange
        var expectedFields = new List<FieldDto>
            {
                new FieldDto { FieldId = 1, Name = "Field1", Label = "Label1", Description = "Desc1" },
                new FieldDto { FieldId = 2, Name = "Field2", Label = "Label2", Description = "Desc2" }
            };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFields);

        var request = new FieldRequest { Operation = "getall" };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Field1", result);
        Assert.Contains("Field2", result);
        Assert.Contains("Label1", result);
        Assert.Contains("Label2", result);
    }

    [Fact]
    public async Task FunctionHandler_Get_WithValidId_ReturnsFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto { FieldId = 1, Name = "Test Field", Label = "Test Label", Description = "Test Description" };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new FieldRequest { Operation = "get", Id = 1 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Test Field", result);
        Assert.Contains("Test Label", result);
    }

    [Fact]
    public async Task FunctionHandler_Get_WithInvalidId_ReturnsNotFoundMessage()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FieldDto?)null);

        var request = new FieldRequest { Operation = "get", Id = 999 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Field not found", result);
    }

    [Fact]
    public async Task FunctionHandler_Get_WithoutId_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest { Operation = "get" };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("ID is required for get operation", result);
    }

    [Fact]
    public async Task FunctionHandler_Create_WithValidData_ReturnsCreatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto { FieldId = 1, Name = "New Field", Label = "New Label", Description = "New Description" };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new FieldRequest
        {
            Operation = "create",
            Name = "New Field",
            Label = "New Label",
            Description = "New Description"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("New Field", result);
        Assert.Contains("New Label", result);
    }

    [Fact]
    public async Task FunctionHandler_Create_WithNullName_UsesDefaultName()
    {
        // Arrange
        var expectedField = new FieldDto { FieldId = 1, Name = "Default Name", Label = "Test Label" };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new FieldRequest
        {
            Operation = "create",
            Name = null,
            Label = "Test Label"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Default Name", result);
    }

    [Fact]
    public async Task FunctionHandler_Update_WithValidData_ReturnsUpdatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto { FieldId = 1, Name = "Updated Field", Label = "Updated Label", Description = "Updated Description" };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new FieldRequest
        {
            Operation = "update",
            Id = 1,
            Name = "Updated Field",
            Label = "Updated Label",
            Description = "Updated Description"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Updated Field", result);
        Assert.Contains("Updated Label", result);
    }

    [Fact]
    public async Task FunctionHandler_Update_WithoutId_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest
        {
            Operation = "update",
            Name = "Test Field",
            Label = "Test Label"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("ID is required for update operation", result);
    }

    [Fact]
    public async Task FunctionHandler_Delete_WithValidId_ReturnsSuccessMessage()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Unit.Value);

        var request = new FieldRequest { Operation = "delete", Id = 1 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Field deleted successfully", result);
    }

    [Fact]
    public async Task FunctionHandler_Delete_WithoutId_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest { Operation = "delete" };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("ID is required for delete operation", result);
    }

    [Fact]
    public async Task FunctionHandler_InvalidOperation_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest { Operation = "invalid" };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Invalid operation. Use 'create', 'get', 'getall', 'update', or 'delete'", result);
    }

    [Fact]
    public async Task FunctionHandler_NullOperation_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest { Operation = null };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Invalid operation. Use 'create', 'get', 'getall', 'update', or 'delete'", result);
    }

    [Fact]
    public async Task FunctionHandler_NullRequest_ReturnsErrorMessage()
    {
        // Arrange
        var request = new FieldRequest { Operation = null };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Invalid operation. Use 'create', 'get', 'getall', 'update', or 'delete'", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsErrorMessage()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var request = new FieldRequest { Operation = "getall" };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("Database connection failed", result);
    }
}