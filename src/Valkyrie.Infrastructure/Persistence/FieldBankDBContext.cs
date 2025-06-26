using Microsoft.EntityFrameworkCore;
using Valkyrie.Domain.Entities;

namespace Valkyrie.Infrastructure.Persistence;

public class ValkyrieDbContext : DbContext
{
    public ValkyrieDbContext(DbContextOptions<ValkyrieDbContext> options)
        : base(options)
    {
    }

    public DbSet<Field> Fields { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ValkyrieDbContext).Assembly);
    }
}
