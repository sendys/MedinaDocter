using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MedinaDokter.Api.Features.Auth;
using MedinaDokter.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MedinaDokter.Api.Infrastructure.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public AuthService(
        AppDbContext dbContext,
        IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
    }

    public async Task<AuthResult> AuthenticateAsync(
        string username,
        string password,
        CancellationToken ct = default)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .Include(x => x.Pegawai)
            .FirstOrDefaultAsync(
                x => x.Username == username,
                ct);

        if (user is null)
        //return null;
        {
            return AuthResult.Failure(
            401,
            "Invalid username or password.");
        }

        if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
        {
            return AuthResult.Failure(
            401,
            "Invalid username or password.");
        }

        var role = user.Pegawai?.Flag;

        if (string.IsNullOrEmpty(role))
        {
            return AuthResult.Failure(
            403,
            "User is not assigned to any valid role.");
        }

        if (role != "dokter" && role != "admin")
        {
            return AuthResult.Failure(
                403,
                "Akun Anda tidak memiliki akses ke aplikasi MedinaDokter.");
        }

        var response = BuildResponse(
            user.Id,
            user.Username,
            user.PegawaiId,
            role);

        return AuthResult.Success(response);
    }

    private AuthResponse BuildResponse(
        ulong userId,
        string username,
        ulong? pegawaiId,
        string role)
    {
        var key = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer is missing");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience is missing");

        if (!int.TryParse(
                _configuration["Jwt:ExpireMinutes"],
                out var expireMinutes) || expireMinutes <= 0)
        {
            throw new InvalidOperationException(
                "Jwt:ExpireMinutes must be a positive integer.");
        }

        var now = DateTime.UtcNow;
        var expiresAt = now.AddMinutes(expireMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new("username", username),
            new(ClaimTypes.Role, role)
        };

        if (pegawaiId.HasValue)
        {
            claims.Add(new Claim("pegawai_id", pegawaiId.Value.ToString()));
        }

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new AuthResponse
        {
            UserId = userId,
            Username = username,
            PegawaiId = pegawaiId,
            Token = tokenString,
            ExpiresAt = expiresAt
        };
    }
}