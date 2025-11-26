using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace E_commerce.Filters
{
    public class ValidationActionFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationActionFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments)
            {
                if (arg.Value == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(arg.Value.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;
                if (validator == null) continue;

                var validationContext = new ValidationContext<object>(arg.Value);
                var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);
                if (!result.IsValid)
                {
                    var errors = result.Errors
                        .GroupBy(e => string.IsNullOrWhiteSpace(e.PropertyName) ? string.Empty : e.PropertyName)
                        .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());

                    context.Result = new ObjectResult(new ValidationProblemDetails(errors))
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return;
                }
            }

            await next();
        }
    }
}
