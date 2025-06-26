using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Valkyrie.Infrastructure.Persistence;

public class ValkyrieDbContextFactory : IDesignTimeDbContextFactory<ValkyrieDBContext>
{
    public ValkyrieDBContext CreateDbContext(string[] args)
    {
        // Looks for appsettings.json in the Functions project
        var functionsProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Valkyrie.Functions");
        var appsettingsPath = Path.Combine(functionsProjectPath, "appsettings.json");

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ValkyrieDBContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseNpgsql(connectionString);

        return new ValkyrieDBContext(optionsBuilder.Options);
    }
}