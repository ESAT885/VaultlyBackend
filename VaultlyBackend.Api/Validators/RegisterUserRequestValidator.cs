using FluentValidation;
using VaultlyBackend.Api.Models;
using VaultlyBackend.Api.Models.Auth;

namespace VaultlyBackend.Api.Validators
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator()
        {
            RuleFor(x => x.UserName)
              .NotEmpty().WithMessage("İsim boş olamaz");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Şifre en az 6 karakter olmalı");
        }
    }
}
