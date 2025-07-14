using Xunit;
using Valkyrie.Domain.Entities;

namespace Valkyrie.Domain.Tests;

public class FieldTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        var field = new Field
        {
            Name = "Test Field",
            Label = "Test Label",
            Description = "Test Description",
            CategoryId = 1,
            FieldTypeId = 2
        };

        Assert.Equal("Test Field", field.Name);
        Assert.Equal("Test Label", field.Label);
        Assert.Equal("Test Description", field.Description);
        Assert.Equal(1, field.CategoryId);
        Assert.Equal(2, field.FieldTypeId);
    }
} 