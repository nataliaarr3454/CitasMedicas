using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using CitasMedicas.Core.CustomEntities;

namespace CitasMedicas.Infrastructure.Validators
{
    public class GetByIdRequestValidator : AbstractValidator<GetByIdRequest>
    {
        public GetByIdRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotNull().WithMessage("El ID es requerido")
                .GreaterThan(0).WithMessage("El ID debe ser mayor a 0")
                .LessThanOrEqualTo(1000000).WithMessage("El ID no puede ser mayor a 1,000,000");
        }
    }
}