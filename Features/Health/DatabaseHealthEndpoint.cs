
using MedinaDokter.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedinaDokter.Api.Features.Health;

public static class DatabaseHealthEndpoint
{
     public static IEndpointRouteBuilder MapDatabaseHealthEndpoint(
         this IEndpointRouteBuilder app)
     {
          app.MapGet("/api/health/database", async (
              AppDbContext db,
              CancellationToken ct) =>
          {
               try
               {
                    var userCount = await db.Users.CountAsync(ct);

                    return Results.Ok(new
                    {
                         status = "Connected",
                         database = "SIMRS",
                         userCount
                    });
               }
               catch (Exception)
               {
                    return Results.Problem(
                      title: "Database connection failed",
                      statusCode: StatusCodes.Status503ServiceUnavailable);
               }
          })
          .AllowAnonymous()
          .WithTags("Health")
          .WithSummary("Check SIMRS database connection");

          return app;
     }
}