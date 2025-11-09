using CitasMedicas.Api.Responses;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisponibilidadController : ControllerBase
    {
        private readonly IDisponibilidadService _service;
        private readonly IValidator<DisponibilidadDto> _validator;

        public DisponibilidadController(IDisponibilidadService service, IValidator<DisponibilidadDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] DisponibilidadDto dto)
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

            var disponibilidad = new Disponibilidad
            {
                MedicoId = dto.MedicoId,
                Fecha = dto.Fecha,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                CostoCita = dto.CostoCita
            };

            var registro = await _service.RegistrarDisponibilidadAsync(disponibilidad);

            if (registro == null)
                return BadRequest(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "El horario se solapa con otra disponibilidad." } }
                ));

            return StatusCode(201, new ApiResponse<Disponibilidad>(
                registro,
                new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Disponibilidad registrada correctamente." } }
            ));
        }

        [HttpGet("listar/{medicoId}")]
        public async Task<IActionResult> Listar(int medicoId)
        {
            var lista = await _service.ObtenerDisponibilidadesPorMedicoAsync(medicoId);
            return Ok(new ApiResponse<IEnumerable<Disponibilidad>>(
                lista,
                new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Lista de disponibilidades del médico obtenida correctamente." } }
            ));
        }

        [HttpGet("filtrar")]
        public async Task<IActionResult> Filtrar([FromQuery] DisponibilidadQueryFilter filters)
        {
            var lista = await _service.ObtenerDisponibilidadesFiltradasAsync(filters);
            return Ok(new ApiResponse<IEnumerable<Disponibilidad>>(
                lista,
                new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Lista de disponibilidades filtradas correctamente." } }
            ));
        }
    }
}
