using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CitasMedicas.Core.DTOs;

namespace CitasMedicas.Infrastructure.Validators
{
    public class CancelarCitaDtoValidator : AbstractValidator<CancelarCitaDto>
    {
        public CancelarCitaDtoValidator()
        {
            RuleFor(c => c.MotivoCancelacion)
                .MaximumLength(200).WithMessage("El motivo no debe exceder los 200 caracteres")
        .When(c => !string.IsNullOrEmpty(c.MotivoCancelacion));
                }
    }
}
