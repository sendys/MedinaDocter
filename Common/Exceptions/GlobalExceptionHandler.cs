
using MedinaDokter.Api.Common.Responses;
using Microsoft.AspNetCore.Diagnostics;

namespace MedinaDokter.Api.Common.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
     private readonly ILogger<GlobalExceptionHandler> _logger;

     public GlobalExceptionHandler(
         ILogger<GlobalExceptionHandler> logger)
     {
          _logger = logger;
     }

     public async ValueTask<bool> TryHandleAsync(
         HttpContext httpContext,
         Exception exception,
         CancellationToken cancellationToken)
     {
          _logger.LogError(
              exception,
              "Unhandled exception occurred. TraceId: {TraceId}",
              httpContext.TraceIdentifier);

          if (httpContext.Response.HasStarted)
          {
               return false;
          }

          httpContext.Response.StatusCode =
              StatusCodes.Status500InternalServerError;

          var response = ApiResponse<object>.Fail(
              "Terjadi kesalahan pada server. Silakan coba kembali.");

          await httpContext.Response.WriteAsJsonAsync(
              response,
              cancellationToken);

          return true;
     }
}