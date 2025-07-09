using Xunit;
using Moq;
using AutoMapper;
using Valkyrie.Application.Features.FieldTypes.Queries.GetAllFieldTypes;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Domain.Entities;
using Valkyrie.Application.Common.DTOs;
using Assert = Xunit.Assert;

public class GetAllFieldTypesQueryHandlerTests
{
    private readonly Mock<IFieldTypeRepository> _mockRepo;
    private readonly IMapper _mapper;
    private readonly GetAllFieldTypesQueryHandler _handler;

    public GetAllFieldTypesQueryHandlerTests()
    {
        _mockRepo = new Mock<IFieldTypeRepository>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<FieldType, FieldTypeDto>();
        });
        _mapper = config.CreateMapper();
        _handler = new GetAllFieldTypesQueryHandler(_mockRepo.Object, _mapper);
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