using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace Valkyrie.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ValkyrieDBContext _context;

    public CategoryRepository(ValkyrieDBContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id)
        => await _context.Categories.FindAsync(id);

    public async Task<IEnumerable<Category>> GetAllAsync()
        => await _context.Categories.OrderBy(c => c.Rank).ToListAsync();

    public async Task<Category> CreateAsync(Category category)
    {
        category.CreatedDate = DateTime.UtcNow;
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        category.ModifiedDate = DateTime.UtcNow;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
} 