using E_commerce.Dto;
using E_commerce.Models.Request;
using E_commerce.Services.Request;

namespace E_commerce.Services
{
    public interface IProductService
    {
        Task<PagedList<ProductDto>> GetProductsAsync(GetProductsRequest request, CancellationToken cancellationToken);
        Task<ProductDetailDto> GetProductAsync(int id, CancellationToken cancellationToken);
        Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken);
        Task<ProductDetailDto> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken);
        Task DeleteProductAsync(int id, CancellationToken cancellationToken);
    }
}
