using Coursea.Data.Repositories;
using Coursea.Interfaces;
using Coursea.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Coursea.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("list/{id}")]
        public async Task<IActionResult> GetCategory(int id) 
        {
            var category = await _unitOfWork.CategoryRepo.GetAsync(id);
            return Ok(category);
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _unitOfWork.CategoryRepo.GetAllAsync();
            return Ok(categories);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCategory(Category category)
        {
            var newCate = new Category
            {
                Name = category.Name,
                Description = category.Description,
                Created_at = DateTime.Now,

            };
            _unitOfWork.CategoryRepo.AddAsync(newCate);
            await _unitOfWork.SaveAsync();
            return Ok();
        }
    }
}
