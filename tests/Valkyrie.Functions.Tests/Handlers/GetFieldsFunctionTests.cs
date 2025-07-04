using Xunit;
using Moq;
using MediatR;
using Amazon.Lambda.Core;
using Valkyrie.Functions.Handlers;
using Valkyrie.Application.Features.Fields.Queries.GetAllFields;
using Valkyrie.Application.Common.DTOs;
using Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;
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
    public async Task FunctionHandler_ReturnsAllFields()
    {
        // Arrange
        var expectedFields = new List<FieldDto>
            {
                new FieldDto { FieldId = 1, Name = "Field1", Label = "Label1", Description = "Description1" },
                new FieldDto { FieldId = 2, Name = "Field2", Label = "Label2", Description = "Description2" },
                new FieldDto { FieldId = 3, Name = "Field3", Label = "Label3", Description = "Description3" }
            };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedFields);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Contains(result, f => f.Name == "Field1" && f.Label == "Label1");
        Assert.Contains(result, f => f.Name == "Field2" && f.Label == "Label2");
        Assert.Contains(result, f => f.Name == "Field3" && f.Label == "Label3");
    }

    [Fact]
    public async Task FunctionHandler_EmptyList_ReturnsEmptyEnumerable()
    {
        // Arrange
        var emptyFields = new List<FieldDto>();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyFields);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_Throws()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldsQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _function.FunctionHandler(_mockContext.Object));
    }
}

public class GetFieldTypesFunctionTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly Mock<ILambdaContext> _mockContext;
    private readonly GetFieldTypesFunction _function;

    public GetFieldTypesFunctionTests()
    {
        _mockMediator = new Mock<IMediator>();
        _mockContext = new Mock<ILambdaContext>();
        var mockLambdaLogger = new Mock<ILambdaLogger>();
        _mockContext.Setup(c => c.Logger).Returns(mockLambdaLogger.Object);
        _function = new GetFieldTypesFunction(_mockMediator.Object);
    }

    [Fact]
    public async Task FunctionHandler_ReturnsAllFieldTypes()
    {
        // Arrange
        var expectedTypes = new List<FieldTypeDto>
        {
            new FieldTypeDto { FieldTypeId = 1, Type = "Date", Structure = "{}" },
            new FieldTypeDto { FieldTypeId = 2, Type = "Text", Structure = "{}" }
        };
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldTypesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTypes);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, t => t.Type == "Date");
        Assert.Contains(result, t => t.Type == "Text");
    }

    [Fact]
    public async Task FunctionHandler_EmptyList_ReturnsEmptyEnumerable()
    {
        // Arrange
        var emptyTypes = new List<FieldTypeDto>();
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldTypesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyTypes);

        // Act
        var result = await _function.FunctionHandler(_mockContext.Object);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task FunctionHandler_WhenMediatorThrowsException_Throws()
    {
        // Arrange
        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetAllFieldTypesQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _function.FunctionHandler(_mockContext.Object));
    }
}