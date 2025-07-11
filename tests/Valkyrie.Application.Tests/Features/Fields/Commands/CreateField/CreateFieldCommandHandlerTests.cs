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
        var mockCategoryRepo = new Mock<ICategoryRepository>();
        var mockFieldTypeRepo = new Mock<IFieldTypeRepository>();
        var fieldMapper = new FieldMapper();
        var fieldCommandMapper = new FieldCommandMapper();
        var mockLogger = new Mock<ILogger<CreateFieldCommandHandler>>();

        var command = new CreateFieldCommand
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = "Test Description",
            CategoryId = 1,
            FieldTypeId = 1
        };

        var category = new Category { CategoryId = 1, Name = "Test Category" };
        var fieldType = new FieldType { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text, Structure = "{}" };

        var fieldEntity = new Field
        {
            FieldId = 1,
            Name = command.Name,
            Label = command.Label,
            Description = command.Description,
            CategoryId = 1,
            Category = category,
            FieldTypeId = 1,
            FieldType = fieldType
        };
        var fieldDto = new FieldDto
        {
            FieldId = 1,
            Name = command.Name,
            Label = command.Label,
            Description = command.Description,
            CategoryId = 1,
            Category = new CategoryDto { CategoryId = 1, Name = "Test Category" },
            FieldTypeId = 1,
            FieldType = new FieldTypeDto { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text.ToString(), Structure = "{}" }
        };

        mockCategoryRepo.Setup(r => r.GetByIdAsync(command.CategoryId)).ReturnsAsync(category);
        mockFieldTypeRepo.Setup(r => r.GetByIdAsync(command.FieldTypeId)).ReturnsAsync(fieldType);
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Field>())).ReturnsAsync(fieldEntity);

        var handler = new CreateFieldCommandHandler(
            mockRepo.Object,
            mockCategoryRepo.Object,
            mockFieldTypeRepo.Object,
            fieldMapper,
            fieldCommandMapper,
            mockLogger.Object
        );

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(fieldDto.FieldId, result.FieldId);
        Assert.Equal(fieldDto.Name, result.Name);
        Assert.Equal(fieldDto.Label, result.Label);
        Assert.Equal(fieldDto.Description, result.Description);
        Assert.Equal(fieldDto.CategoryId, result.CategoryId);
        Assert.NotNull(result.Category);
        Assert.Equal(fieldDto.Category.Name, result.Category!.Name);
        Assert.Equal(fieldDto.FieldTypeId, result.FieldTypeId);
        Assert.NotNull(result.FieldType);
        Assert.Equal(fieldDto.FieldType.Type, result.FieldType!.Type);
    }
}