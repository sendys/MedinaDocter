using System.Text;

using System.Security.Claims;
using MedinaDokter.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MedinaDokter.Api.Common.Responses;

namespace MedinaDokter.Api.Features.Dokter.Me;

public static class DokterMeEndpoint
{
     public static IEndpointRouteBuilder MapDokterMeEndpoint(
         this IEndpointRouteBuilder app)
     {
          app.MapGet("/api/dokter/me", async (
              ClaimsPrincipal user,
              AppDbContext dbContext,
              CancellationToken ct) =>
          {
               // Ambil pegawai_id dari JWT
               var pegawaiIdClaim = user.FindFirst("pegawai_id")?.Value;

               if (!ulong.TryParse(pegawaiIdClaim, out var pegawaiId))
               {
                    return Results.Unauthorized();
               }

               // Ambil data pegawai langsung dari database
               var dokter = await dbContext.Pegawai
                 .AsNoTracking()
                 .Where(p => p.Id == pegawaiId)
                 .Select(p => new
                 {
                      p.Id,
                      p.Nama,
                      p.Flag
                 })
                 .FirstOrDefaultAsync(ct);

               if (dokter is null)
               {
                    return Results.NotFound(
                         ApiResponse<object>.Fail(
                              "Data pegawai tidak ditemukan."));
               }

               if (dokter.Flag != "dokter" && dokter.Flag != "admin")
               {
                    return Results.Json(
                        ApiResponse<object>.Fail("Akses hanya diperuntukkan bagi Dokter DPJP."),
                        statusCode: StatusCodes.Status403Forbidden);
               }

               return Results.Ok(
               ApiResponse<object>.Ok(dokter,
                    "Profil dokter berhasil diambil."));

          })
          .RequireAuthorization("MedinaAccess") // Pastikan hanya dokter yang bisa mengakses endpoint ini
          .WithTags("Dokter")
          .WithSummary("Get logged-in doctor's profile");

          return app;
     }
}