using E_commerce.Models.Request;
using E_commerce.Services.Dto;

namespace E_commerce.Services
{
    public interface ICategoryService
    {
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken);
    }
}
