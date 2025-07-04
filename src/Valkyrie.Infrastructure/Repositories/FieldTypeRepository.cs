using Valkyrie.Domain.Entities;
using Valkyrie.Domain.Interfaces;
using Valkyrie.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Valkyrie.Infrastructure.Repositories;

public class FieldTypeRepository : IFieldTypeRepository
{
    private readonly ValkyrieDBContext _context;

    public FieldTypeRepository(ValkyrieDBContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<FieldType>> GetAllAsync()
    {
        return await _context.FieldTypes.OrderBy(ft => ft.FieldTypeId).ToListAsync();
    }
} 