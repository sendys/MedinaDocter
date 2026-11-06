using FluentValidation;
using MedinaDokter.Api.Common.Responses;

namespace MedinaDokter.Api.Common.Validation;

public class ValidationFilter<T> : IEndpointFilter
    where T : class
{
     public async ValueTask<object?> InvokeAsync(
         EndpointFilterInvocationContext context,
         EndpointFilterDelegate next)
     {
          var validator = context.HttpContext.RequestServices
              .GetService<IValidator<T>>();

          if (validator is null)
          {
               throw new InvalidOperationException(
                   $"Validator for {typeof(T).Name} is not registered.");
          }

          var request = context.Arguments
              .OfType<T>()
              .FirstOrDefault();

          if (request is null)
          {
               return await next(context);
          }

          var validationResult = await validator.ValidateAsync(
              request,
              context.HttpContext.RequestAborted);

          if (!validationResult.IsValid)
          {
               var errors = validationResult.Errors
                   .Select(x => x.ErrorMessage)
                   .Distinct();

               return Results.BadRequest(
                   ApiResponse<object>.Fail(
                       string.Join(" ", errors)));
          }

          return await next(context);
     }
}