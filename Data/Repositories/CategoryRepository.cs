using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.EntityFrameworkCore;

namespace Coursea.Data.Repositories
{
    public class CategoryRepository : ICategoryRepo
    {
        private readonly DataContext _dataContext;
        public CategoryRepository(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }
        public void AddAsync(Category category)
        {
            _dataContext.Categories.Add(category);
        }

        public void DeleteAsync(int id)
        {
            var category = _dataContext.Categories.FirstOrDefault(c => c.Id == id);
            _dataContext.Categories.Remove(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _dataContext.Categories.ToListAsync();
        }

        public async Task<Category> GetAsync(int id)
        {
            return await _dataContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
