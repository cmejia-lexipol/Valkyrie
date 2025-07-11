using Valkyrie.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valkyrie.Domain.Interfaces;

public interface IFieldTypeRepository
{
    Task<IEnumerable<FieldType>> GetAllAsync();
    Task<FieldType?> GetByIdAsync(int id);
}