using E_commerce.Services;
using E_commerce.Models.Request;
using Microsoft.AspNetCore.Mvc;
using E_commerce.Services.Dto;

namespace E_commerce.Controllers
{
    // Simple CategoriesController with only creation for Products
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var result = await _categoryService.CreateCategoryAsync(request, cancellationToken);
            return CreatedAtAction(nameof(CreateCategory), new { id = result.Id }, result);
        }
    }
}
