using MedinaDokter.Api.Features.Auth;

namespace MedinaDokter.Api.Infrastructure.Auth;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(
       string username,
       string password,
       CancellationToken ct = default);
}
