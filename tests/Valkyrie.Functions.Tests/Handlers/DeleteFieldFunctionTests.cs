using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using Valkyrie.Functions.Handlers;
using Valkyrie.Application.Features.Fields.Commands.DeleteField;
using Assert = Xunit.Assert;

namespace Valkyrie.Functions.Tests.Handlers;
public class DeleteFieldFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly DeleteFieldFunction _function;

    public DeleteFieldFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();
        var mockLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLogger.Object);
        _function = new DeleteFieldFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_WithValidId_ReturnsSuccessMessage()
    {
        // Arrange
        var request = new Valkyrie.Functions.Handlers.DeleteFieldRequest { Id = 1 }; // Updated to use the correct namespace
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteFieldCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(MediatR.Unit.Value);

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Field deleted successfully", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsArgumentException_ReturnsValidationError()
    {
        // Arrange
        var request = new Valkyrie.Functions.Handlers.DeleteFieldRequest { Id = 0 }; // Updated to use the correct namespace
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteFieldCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.ArgumentException("Id is required"));

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Validation error:", result);
        Assert.Contains("Id is required", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsError()
    {
        // Arrange
        var request = new Valkyrie.Functions.Handlers.DeleteFieldRequest { Id = 1 }; // Updated to use the correct namespace
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteFieldCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("DB error", result);
    }
}