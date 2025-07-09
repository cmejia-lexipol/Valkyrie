using Valkyrie.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Valkyrie.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
    Task<Category> CreateAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task DeleteAsync(int id);
}