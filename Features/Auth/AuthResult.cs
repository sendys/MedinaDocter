
namespace MedinaDokter.Api.Features.Auth;

public class AuthResult
{
     public bool Succeeded { get; init; }
     public string? ErrorMessage { get; init; }
     public int StatusCode { get; init; }
     public AuthResponse? Data { get; init; }

     public static AuthResult Success(AuthResponse data) =>
         new()
         {
              Succeeded = true,
              StatusCode = 200,
              Data = data
         };

     public static AuthResult Failure(
         int statusCode,
         string message) =>
         new()
         {
              Succeeded = false,
              StatusCode = statusCode,
              ErrorMessage = message
         };
}