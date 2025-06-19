using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace FieldBank.Infrastructure.Persistence;

public class FieldBankDBContextFactory : IDesignTimeDbContextFactory<FieldBankDBContext>
{
    public FieldBankDBContext CreateDbContext(string[] args)
    {
        // Looks for appsettings.json in the Functions project
        var functionsProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FieldBank.Functions");
        var appsettingsPath = Path.Combine(functionsProjectPath, "appsettings.json");
        
        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<FieldBankDBContext>();
        
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        
        optionsBuilder.UseNpgsql(connectionString);

        return new FieldBankDBContext(optionsBuilder.Options);
    }
} 