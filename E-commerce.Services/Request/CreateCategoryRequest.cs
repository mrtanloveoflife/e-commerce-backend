using E_commerce.Services;
using FluentValidation;

namespace E_commerce.Models.Request
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class CreateCategoryRequestValidator : AbstractValidator<CreateCategoryRequest>
    {
        public CreateCategoryRequestValidator(AppDbContext _context)
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(1000);
        }
    }
}
