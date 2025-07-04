using Xunit;
using Valkyrie.Infrastructure.Persistence;
using Valkyrie.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Assert = Xunit.Assert;
using Valkyrie.Domain.Enums;

namespace Valkyrie.Infrastructure.Repositories.Tests;
public class FieldRepositoryTests
{
    private ValkyrieDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ValkyrieDBContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new ValkyrieDBContext(options);
    }

    [Fact]
    public async Task CreateAsync_AddsFieldToDb()
    {
        var context = GetInMemoryDbContext();
        var fieldType = new FieldType { Type = FieldTypeEnum.Text, Structure = "{}" };
        context.FieldTypes.Add(fieldType);
        var category = new Category { Name = "Cat1", Rank = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "TestField", Label = "TestLabel", Description = "Desc", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId };

        var result = await repo.CreateAsync(field);

        Assert.NotNull(result);
        Assert.True(result.FieldId > 0);
        Assert.Equal("TestField", result.Name);
        Assert.Equal("TestLabel", result.Label);
        Assert.Equal("Desc", result.Description);
        Assert.Single(context.Fields);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsField()
    {
        var context = GetInMemoryDbContext();
        var fieldType = new FieldType { Type = FieldTypeEnum.Text, Structure = "{}" };
        context.FieldTypes.Add(fieldType);
        var category = new Category { Name = "Cat1", Rank = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repo = new FieldRepository(context);
        var field = new Field { Name = "Field1", Label = "Label1", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId };
        context.Fields.Add(field);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(field.FieldId);

        Assert.NotNull(result);
        Assert.Equal(field.Name, result.Name);
        Assert.NotNull(result.Category);
        Assert.Equal(category.Name, result.Category.Name);
        Assert.NotNull(result.FieldType);
        Assert.Equal(fieldType.Type, result.FieldType.Type);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFieldsOrderedByName()
    {
        var context = GetInMemoryDbContext();
        var fieldType = new FieldType { Type = FieldTypeEnum.Text, Structure = "{}" };
        context.FieldTypes.Add(fieldType);
        var category = new Category { Name = "Cat1", Rank = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var repo = new FieldRepository(context);
        context.Fields.AddRange(
            new Field { Name = "B", Label = "L2", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId },
            new Field { Name = "A", Label = "L1", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId }
        );
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("B", result[1].Name);
        Assert.All(result, f => Assert.NotNull(f.Category));
        Assert.All(result, f => Assert.NotNull(f.FieldType));
    }

    [Fact]
    public async Task UpdateAsync_UpdatesField()
    {
        var context = GetInMemoryDbContext();
        var fieldType = new FieldType { Type = FieldTypeEnum.Text, Structure = "{}" };
        context.FieldTypes.Add(fieldType);
        var category = new Category { Name = "Cat1", Rank = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "Old", Label = "L", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId };
        context.Fields.Add(field);
        await context.SaveChangesAsync();

        field.Name = "New";
        var updated = await repo.UpdateAsync(field);

        Assert.Equal("New", updated.Name);
        Assert.Equal("New", context.Fields.First().Name);
    }

    [Fact]
    public async Task DeleteAsync_RemovesField()
    {
        var context = GetInMemoryDbContext();
        var fieldType = new FieldType { Type = FieldTypeEnum.Text, Structure = "{}" };
        context.FieldTypes.Add(fieldType);
        var category = new Category { Name = "Cat1", Rank = 1 };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "ToDelete", Label = "L", CategoryId = category.CategoryId, FieldTypeId = fieldType.FieldTypeId };
        context.Fields.Add(field);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(field.FieldId);

        Assert.Empty(context.Fields);
    }
}

public class FieldTypeRepositoryTests
{
    private ValkyrieDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ValkyrieDBContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new ValkyrieDBContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFieldTypesOrderedById()
    {
        var context = GetInMemoryDbContext();
        context.FieldTypes.AddRange(
            new FieldType { FieldTypeId = 2, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Text, Structure = "{}" },
            new FieldType { FieldTypeId = 1, Type = Valkyrie.Domain.Enums.FieldTypeEnum.Date, Structure = "{}" }
        );
        await context.SaveChangesAsync();
        var repo = new FieldTypeRepository(context);

        var result = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].FieldTypeId);
        Assert.Equal(Valkyrie.Domain.Enums.FieldTypeEnum.Date, result[0].Type);
        Assert.Equal(2, result[1].FieldTypeId);
        Assert.Equal(Valkyrie.Domain.Enums.FieldTypeEnum.Text, result[1].Type);
    }

    [Fact]
    public async Task GetAllAsync_Empty_ReturnsEmpty()
    {
        var context = GetInMemoryDbContext();
        var repo = new FieldTypeRepository(context);

        var result = (await repo.GetAllAsync()).ToList();

        Assert.Empty(result);
    }
}