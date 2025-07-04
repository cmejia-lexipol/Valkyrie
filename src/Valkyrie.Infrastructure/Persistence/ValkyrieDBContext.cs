using Microsoft.EntityFrameworkCore;
using Valkyrie.Domain.Entities;

namespace Valkyrie.Infrastructure.Persistence;

public class ValkyrieDBContext : DbContext
{
    public ValkyrieDBContext(DbContextOptions<ValkyrieDBContext> options)
        : base(options)
    {
    }

    public DbSet<Field> Fields { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<FieldType> FieldTypes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValkyrieDBContext).Assembly);
    }
}
