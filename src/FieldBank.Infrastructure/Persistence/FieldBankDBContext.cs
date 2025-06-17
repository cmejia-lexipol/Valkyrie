using Microsoft.EntityFrameworkCore;
using FieldBank.Domain.Entities;

namespace FieldBank.Infrastructure.Persistence;

public class FieldBankDBContext : DbContext
{
    public FieldBankDBContext(DbContextOptions<FieldBankDBContext> options)
        : base(options)
    {
    }

    public DbSet<Field> Fields { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FieldBankDBContext).Assembly);
    }
}
