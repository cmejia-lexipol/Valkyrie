using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using Valkyrie.Functions.Handlers;
using Valkyrie.Application.Features.Fields.Queries.GetAllFields;
using Valkyrie.Application.Common.DTOs;
using Assert = Xunit.Assert;

namespace Valkyrie.Functions.Tests.Handlers;
public class GetFieldsFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly GetFieldsFunction _function;

    public GetFieldsFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();

        // Setup the Logger to avoid NullReferenceException
        var mockLambdaLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLambdaLogger.Object);

        _function = new GetFieldsFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsAllFieldsAsJson()
    {
        // Arrange
        var expectedFields = new List<FieldDto>
            {
                new FieldDto { Id = 1, Name = "Field1", Label = "Label1", Description = "Description1" },
                new FieldDto { Id = 2, Name = "Field2", Label = "Label2", Description = "Description2" },
                new FieldDto { Id = 3, Name = "Field3", Label = "Label3", Description = "Description3" }
            };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFields);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Contains("Field1", result);
        Assert.Contains("Field2", result);
        Assert.Contains("Field3", result);
        Assert.Contains("Label1", result);
        Assert.Contains("Label2", result);
        Assert.Contains("Label3", result);
    }

    [Fact]
    public async Task FunctionHandler_EmptyList_ReturnsEmptyArray()
    {
        // Arrange
        var emptyFields = new List<FieldDto>();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyFields);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.Equal("[]", result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_ReturnsErrorMessage()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.StartsWith("Error:", result);
        Assert.Contains("Database connection failed", result);
    }
}