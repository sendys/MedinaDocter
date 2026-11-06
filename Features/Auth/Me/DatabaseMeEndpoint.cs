using System;
using System.Security.Claims;

namespace MedinaDokter.Api.Features.Auth.Me;

public static class DatabaseMeEndpoint
{
     public static IEndpointRouteBuilder MapDatabaseMeEndpoint(
             this IEndpointRouteBuilder app)
     {
          app.MapGet("/api/auth/me", (ClaimsPrincipal user) =>
          {
               var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst("sub")?.Value;

               var username = user.FindFirst("username")?.Value;

               var pegawaiId = user.FindFirst("pegawai_id")?.Value;

               return Results.Ok(new
               {
                    userId,
                    username,
                    pegawaiId
               });
          })
          .RequireAuthorization()
          .WithTags("Authentication")
          .WithSummary("Get authenticated user information");

          return app;
     }
}
