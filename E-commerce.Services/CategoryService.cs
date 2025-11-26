using E_commerce.Models.Entities;
using E_commerce.Models.Request;
using E_commerce.Services.Dto;

namespace E_commerce.Services
{
    public class CategoryService : ICategoryService
    {
        // Implementation of product-related business logic with EF core goes here
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Name = request.Name,
                Description = request.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }
    }
}
