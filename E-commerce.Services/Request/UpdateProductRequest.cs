using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace E_commerce.Services.Request
{
    public class UpdateProductRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public int CategoryId { get; set; }
        public IFormFile Image { get; set; }
    }

    public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
    {
        public UpdateProductRequestValidator(AppDbContext context)
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
