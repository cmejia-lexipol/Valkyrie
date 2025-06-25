using Xunit;
using FieldBank.Infrastructure.Persistence;
using FieldBank.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Assert = Xunit.Assert;

namespace FieldBank.Infrastructure.Repositories.Tests;
public class FieldRepositoryTests
{
    private FieldBankDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<FieldBankDBContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options;
        return new FieldBankDBContext(options);
    }

    [Fact]
    public async Task CreateAsync_AddsFieldToDb()
    {
        var context = GetInMemoryDbContext();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "TestField", Label = "TestLabel", Description = "Desc" };

        var result = await repo.CreateAsync(field);

        Assert.NotNull(result);
        Assert.True(result.Id > 0);
        Assert.Equal("TestField", result.Name);
        Assert.Equal("TestLabel", result.Label);
        Assert.Equal("Desc", result.Description);
        Assert.Single(context.Fields);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsField()
    {
        var context = GetInMemoryDbContext();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "Field1", Label = "Label1" };
        context.Fields.Add(field);
        await context.SaveChangesAsync();

        var result = await repo.GetByIdAsync(field.Id);

        Assert.NotNull(result);
        Assert.Equal(field.Name, result.Name);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllFieldsOrderedByName()
    {
        var context = GetInMemoryDbContext();
        var repo = new FieldRepository(context);
        context.Fields.AddRange(
            new Field { Name = "B", Label = "L2" },
            new Field { Name = "A", Label = "L1" }
        );
        await context.SaveChangesAsync();

        var result = (await repo.GetAllAsync()).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("A", result[0].Name);
        Assert.Equal("B", result[1].Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesField()
    {
        var context = GetInMemoryDbContext();
        var repo = new FieldRepository(context);
        var field = new Field { Name = "Old", Label = "L" };
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
        var repo = new FieldRepository(context);
        var field = new Field { Name = "ToDelete", Label = "L" };
        context.Fields.Add(field);
        await context.SaveChangesAsync();

        await repo.DeleteAsync(field.Id);

        Assert.Empty(context.Fields);
    }
}