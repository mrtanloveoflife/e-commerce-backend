using E_commerce.Dto;
using E_commerce.Models.Entities;
using E_commerce.Models.Request;
using E_commerce.Services.Dto;
using E_commerce.Services.Request;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.IO;
using System.Text.Json;

namespace E_commerce.Services
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IDistributedCache _cache;

        public ProductService(AppDbContext context
#if !DEBUG
            ,IDistributedCache distributedCache
#endif
        )
        {
            _context = context;
#if !DEBUG
            _cache = distributedCache;
#endif
        }

        public async Task<PagedList<ProductDto>> GetProductsAsync(GetProductsRequest request, CancellationToken cancellationToken)
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(p => p.Name.Contains(request.SearchTerm) || p.Description.Contains(request.SearchTerm));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var productDtos = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    BasePrice = p.BasePrice,
                    CategoryId = p.CategoryId,
                    ImageBase64 = p.Image != null ? Convert.ToBase64String(p.Image) : null,
                })
                .OrderByDescending(p => p.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);
            return new PagedList<ProductDto>
            {
                Items = productDtos,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<ProductDetailDto> GetProductAsync(int id, CancellationToken cancellationToken)
        {
#if !DEBUG
            var cacheKey = $"product_{id}";
            var cachedProduct = await _cache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(cachedProduct))
            {
                return JsonSerializer.Deserialize<ProductDetailDto>(cachedProduct);
            }
#endif

            var product = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product == null) return null;
#if !DEBUG
            await _cache.SetStringAsync(cacheKey,
                JsonSerializer.Serialize(product),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                },
                cancellationToken);
#endif
            var productDto = new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                CategoryId = product.CategoryId,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                } : null,
                ImageBase64 = product.Image != null ? Convert.ToBase64String(product.Image) : null,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
            return productDto;
        }

        public async Task<ProductDetailDto> CreateProductAsync(CreateProductRequest request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                BasePrice = request.BasePrice,
                CategoryId = request.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // If an image is provided, read into byte[]
            if (request.Image != null && request.Image.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await request.Image.CopyToAsync(ms, cancellationToken);
                    product.Image = ms.ToArray();
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);
            await _context.Entry(product).Reference(p => p.Category).LoadAsync(cancellationToken);

            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                CategoryId = product.CategoryId,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                } : null,
                ImageBase64 = product.Image != null ? Convert.ToBase64String(product.Image) : null,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task<ProductDetailDto> UpdateProductAsync(UpdateProductRequest request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
            if (product == null) return null;
            product.Name = request.Name;
            product.Description = request.Description;
            product.BasePrice = request.BasePrice;
            product.CategoryId = request.CategoryId;
            product.UpdatedAt = DateTime.UtcNow;

            // If new image provided, replace existing image
            if (request.Image != null && request.Image.Length > 0)
            {
                using var ms = new MemoryStream();
                await request.Image.CopyToAsync(ms, cancellationToken);
                product.Image = ms.ToArray();
            }

            await _context.SaveChangesAsync(cancellationToken);
            return new ProductDetailDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                CategoryId = product.CategoryId,
                Category = product.Category != null ? new CategoryDto
                {
                    Id = product.Category.Id,
                    Name = product.Category.Name,
                } : null,
                ImageBase64 = product.Image != null ? Convert.ToBase64String(product.Image) : null,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };
        }

        public async Task DeleteProductAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
            if (product != null)
            {
                product.IsActive = false;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
