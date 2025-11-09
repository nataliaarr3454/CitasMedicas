using CitasMedicas.Core.DTOs;
using FluentValidation;

namespace CitasMedicas.Infrastructure.Validators
{
    public class ReservaCitaDtoValidator : AbstractValidator<ReservaCitaDto>
    {
        public ReservaCitaDtoValidator()
        {
            RuleFor(r => r.CorreoPaciente)
                .NotEmpty().WithMessage("El correo del paciente es obligatorio.")
                .EmailAddress().WithMessage("Debe ingresar un correo válido.");

            RuleFor(r => r.DisponibilidadId)
                .GreaterThan(0).WithMessage("La disponibilidad es obligatoria.");

            RuleFor(r => r.Motivo)
                .NotEmpty().WithMessage("El motivo de la cita es obligatorio.")
                .MaximumLength(200).WithMessage("El motivo no debe exceder los 200 caracteres.");
        }
    }

}

