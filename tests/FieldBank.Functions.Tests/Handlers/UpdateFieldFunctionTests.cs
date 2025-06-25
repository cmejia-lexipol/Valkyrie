using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using FieldBank.Functions.Handlers;
using FieldBank.Application.Features.Fields.Commands.UpdateField;
using FieldBank.Application.Common.DTOs;
using Assert = Xunit.Assert;

namespace FieldBank.Functions.Tests.Handlers;
public class UpdateFieldFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly UpdateFieldFunction _function;

    public UpdateFieldFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();
        var mockLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLogger.Object);
        _function = new UpdateFieldFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_WithValidData_ReturnsUpdatedFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto { Id = 1, Name = "Updated Field", Label = "Updated Label", Description = "Updated Desc" };
        var request = new UpdateFieldRequest { Id = 1, Name = "Updated Field", Label = "Updated Label", Description = "Updated Desc" };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedField);

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Contains("Updated Field", result);
        Assert.Contains("Updated Label", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsArgumentException_ReturnsValidationError()
    {
        // Arrange
        var request = new UpdateFieldRequest { Id = 1, Name = "", Label = "Label" };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.ArgumentException("Name is required"));

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Validation error:", result);
        Assert.Contains("Name is required", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsError()
    {
        // Arrange
        var request = new UpdateFieldRequest { Id = 1, Name = "Test", Label = "Label" };
        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateFieldCommand>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("DB error", result);
    }
}