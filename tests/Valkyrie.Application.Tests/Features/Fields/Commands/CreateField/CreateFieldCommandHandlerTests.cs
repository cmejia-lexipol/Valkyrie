using Xunit;
using Moq;
using AutoMapper;
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
        var mockMapper = new Mock<IMapper>();
        var mockLogger = new Mock<ILogger<CreateFieldCommandHandler>>();

        var command = new CreateFieldCommand
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = "Test Description"
        };

        var fieldEntity = new Field { FieldId = 1, Name = command.Name, Label = command.Label, Description = command.Description };
        var fieldDto = new FieldDto { FieldId = 1, Name = command.Name, Label = command.Label, Description = command.Description };

        mockMapper.Setup(m => m.Map<Field>(command)).Returns(fieldEntity);
        mockRepo.Setup(r => r.CreateAsync(fieldEntity)).ReturnsAsync(fieldEntity);
        mockMapper.Setup(m => m.Map<FieldDto>(fieldEntity)).Returns(fieldDto);

        var handler = new CreateFieldCommandHandler(mockRepo.Object, mockMapper.Object, mockLogger.Object);

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