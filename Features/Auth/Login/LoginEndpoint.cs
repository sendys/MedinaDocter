using FluentValidation;
using MedinaDokter.Api.Common.Responses;
using MedinaDokter.Api.Common.Validation;
using MedinaDokter.Api.Infrastructure.Auth;

namespace MedinaDokter.Api.Features.Auth.Login;

public static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLoginEndpoint(
        this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            IAuthService authService,
            CancellationToken ct) =>
        {
            var result = await authService.AuthenticateAsync(
                request.Username,
                request.Password,
                ct);

            if (!result.Succeeded)
            {
                return Results.Json(
                    ApiResponse<object>.Fail(
                        result.ErrorMessage ?? "Login gagal."),
                    statusCode: result.StatusCode);
            }

            return Results.Ok(
                ApiResponse<AuthResponse>.Ok(
                    result.Data!,
                    "Login berhasil."));
        })
        .AddEndpointFilter<ValidationFilter<LoginRequest>>()
        .AllowAnonymous()
        .WithTags("Authentication")
        .WithSummary("Authenticate user and return JWT token")
        .WithDescription("Accepts username and password, validates credentials, and issues a JWT access token.");

        return app;
    }
}