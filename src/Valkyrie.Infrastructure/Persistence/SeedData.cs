using Valkyrie.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Valkyrie.Infrastructure.Persistence;

public static class SeedData
{
    public static void Seed(ValkyrieDBContext context)
    {
        // Seed Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Subject Info", Rank = 1, CreatedDate = DateTime.UtcNow },
                new Category { Name = "Narrative", Rank = 2, CreatedDate = DateTime.UtcNow }
            );
        }

        // Add future seeders here

        context.SaveChanges();
    }
} 