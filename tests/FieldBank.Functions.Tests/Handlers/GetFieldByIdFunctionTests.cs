using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using FieldBank.Functions.Handlers;
using FieldBank.Application.Features.Fields.Queries.GetFieldById;
using FieldBank.Application.Common.DTOs;
using Assert = Xunit.Assert;

namespace FieldBank.Functions.Tests.Handlers;
public class GetFieldByIdFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly GetFieldByIdFunction _function;

    public GetFieldByIdFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();
        var mockLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLogger.Object);
        _function = new GetFieldByIdFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_WithValidId_ReturnsFieldAsJson()
    {
        // Arrange
        var expectedField = new FieldDto { Id = 1, Name = "Field1", Label = "Label1", Description = "Desc1" };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(expectedField);
        var request = new GetFieldByIdRequest { Id = 1 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Contains("Field1", result);
        Assert.Contains("Label1", result);
    }

    [Fact]
    public async Task FunctionHandler_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((FieldDto?)null);
        var request = new GetFieldByIdRequest { Id = 999 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.Equal("Field not found", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsError()
    {
        // Arrange
        _mockMediator.Setup(m => m.Send(It.IsAny<GetFieldByIdQuery>(), It.IsAny<CancellationToken>())).ThrowsAsync(new System.Exception("DB error"));
        var request = new GetFieldByIdRequest { Id = 1 };

        // Act
        var result = await _function.FunctionHandler(request, _mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("DB error", result);
    }
}
