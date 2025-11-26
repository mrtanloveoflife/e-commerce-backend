using E_commerce.Services;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Models.Request
{
    public class CreateProductRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }

        // Image file uploaded as part of the multipart/form-data request
        public IFormFile Image { get; set; }
    }

    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator(AppDbContext context)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.BasePrice).GreaterThan(0);
            // Check if category exists
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .MustAsync(async (categoryId, cancellation) =>
                {
                    var category = await context.Categories.FindAsync(categoryId, cancellation);
                    return category != null;
                }).WithMessage("Category does not exist.");
        }
    }
}
