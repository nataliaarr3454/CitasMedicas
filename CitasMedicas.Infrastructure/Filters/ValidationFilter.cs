using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CitasMedicas.Infrastructure.Filters
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType);
                if (validator == null) continue;

                var validateMethod = validator.GetType()
                    .GetMethods()
                    .FirstOrDefault(m =>
                        m.Name == "ValidateAsync" &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[0].ParameterType == argument.GetType() &&
                        m.GetParameters()[1].ParameterType == typeof(CancellationToken));

                if (validateMethod == null) continue;

                var validationResult = await (Task<FluentValidation.Results.ValidationResult>)
                    validateMethod.Invoke(validator, new[] { argument, CancellationToken.None });

                if (!validationResult.IsValid)
                {
                    context.Result = new BadRequestObjectResult(new { Errors = validationResult.Errors });
                    return;
                }
            }
            await next();
        }
    }
}