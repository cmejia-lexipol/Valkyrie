using Xunit;
using Moq;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Common.DTOs;
using Assert = Xunit.Assert;

public class GetAllFieldTypesQueryHandlerTests
{
    private readonly Mock<IFieldTypeRepository> _mockRepo;
    private readonly FieldTypeMapper _fieldTypeMapper;
    private readonly GetAllFieldTypesQueryHandler _handler;

    public GetAllFieldTypesQueryHandlerTests()
    {
        _mockRepo = new Mock<IFieldTypeRepository>();
        _fieldTypeMapper = new FieldTypeMapper();
        _handler = new GetAllFieldTypesQueryHandler(_mockRepo.Object, _fieldTypeMapper);
    }

    [Fact]
    public async Task Handle_ReturnsAllFieldTypes()
    {
        // Arrange
        var fieldTypes = new List<FieldType>
        {
            new FieldType { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Date, Structure = "{}" },
            new FieldType { FieldTypeId = 2, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text, Structure = "{}" }
        };
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(fieldTypes);

        // Act
        var result = (await _handler.Handle(new GetAllFieldTypesQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal(Valkyrie.Domain.Enums.FieldTypeEnum.Date.ToString(), result[0].Type);
        Assert.Equal(Valkyrie.Domain.Enums.FieldTypeEnum.Text.ToString(), result[1].Type);
    }

    [Fact]
    public async Task Handle_EmptyList_ReturnsEmpty()
    {
        // Arrange
        _mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<FieldType>());

        // Act
        var result = (await _handler.Handle(new GetAllFieldTypesQuery(), CancellationToken.None)).ToList();

        // Assert
        Assert.Empty(result);
    }
}