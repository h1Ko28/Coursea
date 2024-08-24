using Coursea.Models;

namespace Coursea.Interfaces
{
    public interface ICategoryRepo
    {
        Task<Category> GetAsync(int id);
        Task<IEnumerable<Category>> GetAllAsync();
        void AddAsync(Category category);
        void DeleteAsync(int id);
    }
}
