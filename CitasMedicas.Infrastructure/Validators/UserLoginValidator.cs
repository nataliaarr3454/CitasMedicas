using FluentValidation;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Infrastructure.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLogin>
    {
        public UserLoginValidator()
        {
            RuleFor(u => u.Login)
                .NotEmpty().WithMessage("El login es obligatorio.");

            RuleFor(u => u.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.");
        }
    }
}