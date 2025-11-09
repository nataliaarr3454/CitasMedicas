using FluentValidation;
using CitasMedicas.Core.DTOs;

namespace CitasMedicas.Infrastructure.Validators
{
    public class MedicoDtoValidator : AbstractValidator<MedicoDto>
    {
        public MedicoDtoValidator()
        {
            RuleFor(m => m.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

            RuleFor(m => m.Especialidad)
                .NotEmpty().WithMessage("La especialidad es obligatoria.")
                .MaximumLength(80).WithMessage("La especialidad no debe exceder los 80 caracteres.");

            RuleFor(m => m.Correo)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("Debe ingresar un correo válido.");

            RuleFor(m => m.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .Matches(@"^[0-9]{8,12}$").WithMessage("El teléfono debe contener entre 8 y 12 dígitos.");
        }
    }
}
