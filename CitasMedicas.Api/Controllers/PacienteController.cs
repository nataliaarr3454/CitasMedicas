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
    public class PacienteController : ControllerBase
    {
        private readonly IPacienteService _service;
        private readonly IValidator<PacienteDto> _validator;

        public PacienteController(IPacienteService service, IValidator<PacienteDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] PacienteDto dto)
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

            try
            {
                var paciente = await _service.RegistrarPacienteAsync(dto);

                if (paciente == null)
                    return BadRequest(new ApiResponse<string>(
                        default!,
                        new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "El correo ya está registrado." } }
                    ));

                return StatusCode(201, new ApiResponse<PacienteDto>(
                    paciente,
                    new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Paciente registrado correctamente." } }
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = $"Error: {ex.Message}" } }
                ));
            }
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.ObtenerPacientesAsync();
            return Ok(new ApiResponse<IEnumerable<PacienteDto>>(
                lista,
                new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Lista de pacientes obtenida correctamente." } }
            ));
        }
    }
}

