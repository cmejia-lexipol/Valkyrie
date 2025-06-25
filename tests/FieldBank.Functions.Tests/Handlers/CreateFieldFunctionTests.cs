using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using FieldBank.Functions.Handlers;
using FieldBank.Application.Features.Fields.Commands.CreateField;
using FieldBank.Application.Common.DTOs;
using Assert = Xunit.Assert;

namespace FieldBank.Functions.Tests.Handlers;
public class CreateFieldFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly CreateFieldFunction _function;

    public CreateFieldFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();

        // Setup del Logger para evitar NullReferenceException
        var mockLambdaLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLambdaLogger.Object);

        _function = new CreateFieldFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_WithValidData_ReturnsCreatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto
        {
            Id = 1,
            Name = "New Field",
            Label = "New Label",
            Description = "New Description"
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new CreateFieldRequest
        {
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
        Assert.Contains("New Description", result);
    }

    [Fact]
    public async Task FunctionHandler_WithNullDescription_ReturnsCreatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto
        {
            Id = 1,
            Name = "Test Field",
            Label = "Test Label",
            Description = null
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new CreateFieldRequest
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = null
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Test Field", result);
        Assert.Contains("Test Label", result);
    }

    [Fact]
    public async Task FunctionHandler_WithEmptyDescription_ReturnsCreatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto
        {
            Id = 1,
            Name = "Test Field",
            Label = "Test Label",
            Description = ""
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedField);

        var request = new CreateFieldRequest
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = ""
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Test Field", result);
        Assert.Contains("Test Label", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsArgumentException_ReturnsValidationError()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ArgumentException("Field name is required"));

        var request = new CreateFieldRequest
        {
            Name = "",
            Label = "Test Label"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Validation error:", result);
        Assert.Contains("Field name is required", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsErrorMessage()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateFieldCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        var request = new CreateFieldRequest
        {
            Name = "Test Field",
            Label = "Test Label"
        };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("Database connection failed", result);
    }
}
}