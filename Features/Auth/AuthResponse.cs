namespace MedinaDokter.Api.Features.Auth;

public class AuthResponse
{
    public ulong UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    //public string Name { get; set; } = string.Empty;
    public ulong? PegawaiId { get; set; }

    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
