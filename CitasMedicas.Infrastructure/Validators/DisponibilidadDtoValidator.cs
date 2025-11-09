using FluentValidation;
using CitasMedicas.Core.DTOs;
using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Validators
{
    public class DisponibilidadDtoValidator : AbstractValidator<DisponibilidadDto>
    {
        public DisponibilidadDtoValidator()
        {
            RuleFor(d => d.MedicoId)
                .GreaterThan(0)
                .WithMessage("El ID del médico es obligatorio y debe ser válido.");

            RuleFor(d => d.Fecha)
                .NotEmpty().WithMessage("La fecha es obligatoria.")
                .Must(fecha => fecha.Date >= DateTime.Today)
                .WithMessage("La fecha de disponibilidad no puede ser en el pasado.");

            RuleFor(d => d.HoraInicio)
                .NotEmpty().WithMessage("La hora de inicio es obligatoria.");

            RuleFor(d => d.HoraFin)
                .NotEmpty().WithMessage("La hora de fin es obligatoria.")
                .GreaterThan(d => d.HoraInicio)
                .WithMessage("La hora de fin debe ser posterior a la hora de inicio.");

            RuleFor(d => d.CostoCita)
                .GreaterThan(0)
                .WithMessage("El costo de la cita debe ser mayor a 0.");
        }
    }
}
