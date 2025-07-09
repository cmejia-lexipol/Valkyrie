using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Enums;

namespace Valkyrie.Infrastructure.Persistence;

public static class SeedData
{
    public static void Seed(ValkyrieDBContext context)
    {
        // Seed Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Demographics", Rank = 1, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Location", Rank = 2, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Incident Details", Rank = 3, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Injury Details", Rank = 4, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Equipment Details", Rank = 5, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Vehicle details", Rank = 6, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Other", Rank = 7, CreatedDate = DateTime.UtcNow }
            );
        }
        // Add future seeders here

        context.SaveChanges();
    }
}