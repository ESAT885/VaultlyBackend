using FluentValidation;
using VaultlyBackend.Api.Models;

namespace VaultlyBackend.Api.Validators
{
    public class UserDtoValidator : AbstractValidator<UserDto>
    {
        public UserDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("İsim boş olamaz");

            RuleFor(x => x.PasswordHash)
                .NotEmpty()
                .MinimumLength(6)
                .WithMessage("Şifre en az 6 karakter olmalı");


        }
    }
}
