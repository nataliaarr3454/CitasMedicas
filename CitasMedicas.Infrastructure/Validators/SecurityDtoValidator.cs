using FluentValidation;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Enums;

namespace CitasMedicas.Infrastructure.Validators
{
    public class SecurityDtoValidator : AbstractValidator<SecurityDto>
    {
        public SecurityDtoValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

            RuleFor(s => s.Login)
                .NotEmpty().WithMessage("El login es obligatorio.")
                .MaximumLength(100).WithMessage("El login no debe exceder los 100 caracteres.")
                .Matches(@"^[a-zA-Z0-9]+$").WithMessage("El login solo puede contener letras y números.");

            RuleFor(s => s.Password)
                .NotEmpty().WithMessage("La contraseña es obligatoria.")
                .MinimumLength(6).WithMessage("La contraseña debe tener al menos 6 caracteres.")
                .MaximumLength(100).WithMessage("La contraseña no debe exceder los 100 caracteres.");

            RuleFor(s => s.Role)
                .IsInEnum().WithMessage("El rol no es válido.");
        }
    }
}