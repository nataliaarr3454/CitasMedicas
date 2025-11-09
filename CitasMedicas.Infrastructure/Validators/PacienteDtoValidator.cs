using FluentValidation;
using CitasMedicas.Core.DTOs;

namespace CitasMedicas.Infrastructure.Validators
{
    public class PacienteDtoValidator : AbstractValidator<PacienteDto>
    {
        public PacienteDtoValidator()
        {
            RuleFor(p => p.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(100).WithMessage("El nombre no debe exceder los 100 caracteres.");

            RuleFor(p => p.Apellido)
                .NotEmpty().WithMessage("El apellido es obligatorio.")
                .MaximumLength(100).WithMessage("El apellido no debe exceder los 100 caracteres.");

            RuleFor(p => p.Correo)
                .NotEmpty().WithMessage("El correo es obligatorio.")
                .EmailAddress().WithMessage("Debe ingresar un correo válido.");

            RuleFor(p => p.Telefono)
                .NotEmpty().WithMessage("El teléfono es obligatorio.")
                .Matches(@"^[0-9]{8,12}$").WithMessage("El teléfono debe contener entre 8 y 12 dígitos numéricos.");

            RuleFor(p => p.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria.")
                .MaximumLength(200).WithMessage("La dirección no debe exceder los 200 caracteres.");

            RuleFor(p => p.Edad)
                .GreaterThan(0).WithMessage("La edad debe ser mayor que 0.")
                .LessThanOrEqualTo(120).WithMessage("La edad no puede ser mayor a 120 años.");
        }
    }
}
