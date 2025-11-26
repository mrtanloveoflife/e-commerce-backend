using E_commerce.Services.Dto;

namespace E_commerce.Dto
{
    public class ProductDetailDto : ProductDto
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public CategoryDto Category { get; set; }
    }
}
