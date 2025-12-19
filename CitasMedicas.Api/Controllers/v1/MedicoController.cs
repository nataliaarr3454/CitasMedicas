using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CitasMedicas.Api.Controllers.v1
{
    [AllowAnonymous] 
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
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
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validación." }
                    }
                ));
            }

            var medico = await _service.RegistrarMedicoAsync(dto);

            if (medico == null)
                return BadRequest(new ApiResponse<object>(
                    null,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.warning.ToString(), Description = "El correo ya está registrado." }
                    }
                ));

            return StatusCode(201, new ApiResponse<MedicoDto>(
                medico,
                new List<Message>
                {
                    new Message { Type = TypeMessage.success.ToString(), Description = "Médico registrado correctamente." }
                }
            ));
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            var medicos = await _service.ObtenerMedicosAsync();
            return Ok(new ApiResponse<IEnumerable<MedicoDto>>(
                 medicos,
                 new List<Message>
                 {
                    new Message { Type = TypeMessage.information.ToString(), Description = "Lista de médicos obtenida correctamente." }
                 }
             ));
        }






        //get /api/v1/Medico/cantidad-citas
        [HttpGet("cantidad-citas")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCantidadCitasPorMedico()
        {
            try
            {
                var reporte = await _service.GetCantidadCitasPorMedicoAsync();

                return Ok(new ApiResponse<IEnumerable<MedicoCantidadCitasDto>>(
                    reporte,
                    new List<Message>
                    {
                        new Message
                        {
                            Type = TypeMessage.success.ToString(),
                            Description = $"Reporte generado con {reporte.Count()} médicos"
                        }
                    }
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(
                    null,
                    new List<Message>
                    {
                        new Message
                        {
                            Type = TypeMessage.error.ToString(),
                            Description = $"Error al generar reporte: {ex.Message}"
                        }
                    }
                ));
            }
        }
    }
}
