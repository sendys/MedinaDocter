using FluentValidation;

namespace MedinaDokter.Api.Features.Auth.Login;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
     public LoginRequestValidator()
     {
          RuleFor(x => x.Username)
              .NotEmpty()
              .WithMessage("Username wajib diisi.");
          //.MaximumLength(191)
          //.WithMessage("Username maksimal 191 karakter.");

          RuleFor(x => x.Password)
              .NotEmpty()
              .WithMessage("Password wajib diisi.")
              .MinimumLength(6)
              .WithMessage("Password minimal 6 karakter.");
     }
}