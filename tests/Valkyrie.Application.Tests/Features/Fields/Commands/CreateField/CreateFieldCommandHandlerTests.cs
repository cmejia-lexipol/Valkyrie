using Xunit;
using Moq;
using Valkyrie.Application.Common.Mappings;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Application.Common.DTOs;
using Microsoft.Extensions.Logging;
using Assert = Xunit.Assert;

namespace Valkyrie.Application.Features.Fields.Commands.CreateField.Tests;
public class CreateFieldCommandHandlerTests
{
    [Fact()]
    public void HandleTest()
    {

    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsFieldDto()
    {
        // Arrange
        var mockRepo = new Mock<IFieldRepository>();
        var fieldMapper = new FieldMapper();
        var fieldCommandMapper = new FieldCommandMapper();
        var mockLogger = new Mock<ILogger<CreateFieldCommandHandler>>();

        var command = new CreateFieldCommand
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = "Test Description"
        };

        var fieldEntity = new Field {
            FieldId = 1,
            Name = command.Name,
            Label = command.Label,
            Description = command.Description,
            CategoryId = 1,
            Category = new Category { CategoryId = 1, Name = "Test Category" },
            FieldTypeId = 1,
            FieldType = new FieldType { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text, Structure = "{}" }
        };
        var fieldDto = new FieldDto {
            FieldId = 1,
            Name = command.Name,
            Label = command.Label,
            Description = command.Description,
            CategoryId = 1,
            Category = new CategoryDto { CategoryId = 1, Name = "Test Category" },
            FieldTypeId = 1,
            FieldType = new FieldTypeDto { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text.ToString(), Structure = "{}" }
        };

        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Field>())).ReturnsAsync(fieldEntity);

        var handler = new CreateFieldCommandHandler(mockRepo.Object, fieldMapper, fieldCommandMapper, mockLogger.Object);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fieldDto.FieldId, result.FieldId);
        Assert.Equal(fieldDto.Name, result.Name);
        Assert.Equal(fieldDto.Label, result.Label);
        Assert.Equal(fieldDto.Description, result.Description);
    }
}