using Valkyrie.Domain.Entities;

namespace Valkyrie.Domain.Interfaces;

public interface IFieldRepository
{
    Task<Field?> GetByIdAsync(int id);
    Task<Field> CreateAsync(Field field);
    Task<IEnumerable<Field>> GetAllAsync();
    Task<Field> UpdateAsync(Field field);
    Task DeleteAsync(int id);
}