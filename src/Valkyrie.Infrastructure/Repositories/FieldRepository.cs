using Microsoft.EntityFrameworkCore;
using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Infrastructure.Persistence;

namespace Valkyrie.Infrastructure.Repositories;

public class FieldRepository : IFieldRepository
{
    private readonly ValkyrieDBContext _context;

    public FieldRepository(ValkyrieDBContext context)
    {
        _context = context;
    }

    public async Task<Field?> GetByIdAsync(int id)
    {
        return await _context.Fields
            .Include(f => f.Category)
            .FirstOrDefaultAsync(f => f.FieldId == id);
    }

    public async Task<Field> CreateAsync(Field field)
    {
        // Set audit fields
        field.CreatedDate = DateTime.UtcNow;
        field.CreatedBy = "System"; // TODO: Get from current user context

        await _context.Fields.AddAsync(field);
        await _context.SaveChangesAsync();

        return field;
    }

    public async Task<IEnumerable<Field>> GetAllAsync()
    {
        return await _context.Fields
            .Include(f => f.Category)
            .OrderBy(f => f.Name)
            .ToListAsync();
    }

    public async Task<Field> UpdateAsync(Field field)
    {
        // Set audit fields
        field.ModifiedDate = DateTime.UtcNow;
        field.ModifiedBy = "System"; // TODO: Get from current user context

        _context.Fields.Update(field);
        await _context.SaveChangesAsync();

        return field;
    }

    public async Task DeleteAsync(int id)
    {
        var field = await _context.Fields.FindAsync(id);
        if (field != null)
        {
            _context.Fields.Remove(field);
            await _context.SaveChangesAsync();
        }
    }
}