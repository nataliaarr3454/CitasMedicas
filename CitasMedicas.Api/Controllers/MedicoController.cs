using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicoController : ControllerBase
    {
        private readonly IMedicoService _service;
        private readonly IValidator<MedicoDto> _validator;

        public MedicoController(IMedicoService service, IValidator<MedicoDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] MedicoDto dto)
        {
            var validation = await _validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => e.ErrorMessage).ToList();
                return BadRequest(new ApiResponse<object>(
                    new { errores },
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validación." } }
                ));
            }

            var medico = await _service.RegistrarMedicoAsync(dto);

            if (medico == null)
                return BadRequest(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "El correo ya está registrado." } }
                ));

            return StatusCode(201, new ApiResponse<MedicoDto>(
                medico,
                new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Médico registrado correctamente." } }
            ));
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var medicos = await _service.ObtenerMedicosAsync();
            return Ok(new ApiResponse<IEnumerable<MedicoDto>>(
                medicos,
                new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Lista de médicos obtenida correctamente." } }
            ));
        }
    }
}
