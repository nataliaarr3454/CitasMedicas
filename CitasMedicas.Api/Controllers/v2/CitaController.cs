using AutoMapper;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Linq;

namespace CitasMedicas.Api.Controllers.v2
{
    [Authorize]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _service;
        private readonly IValidator<ReservaCitaDto> _validator;
        private readonly IValidator<CancelarCitaDto> _cancelarValidator;
        private readonly IMapper _mapper;

        public CitaController(
            ICitaService service,
            IValidator<ReservaCitaDto> validator,
            IValidator<CancelarCitaDto> cancelarValidator,
            IMapper mapper)
        {
            _service = service;
            _validator = validator;
            _cancelarValidator = cancelarValidator;
            _mapper = mapper;
        }

        [HttpPost("reservar")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Reservar([FromBody] ReservaCitaDto dto)
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

            try
            {
                var cita = await _service.ReservarCitaAsync(dto);

                var respuestaMejorada = new
                {
                    cita.Id,
                    cita.FechaCita,
                    cita.HoraCita,
                    cita.Estado,
                    Medico = new { cita.Medico.Id, cita.Medico.Nombre },
                    Paciente = new { cita.Paciente.Id, cita.Paciente.Nombre },
                    Pago = cita.Pago != null ? new { cita.Pago.Monto, cita.Pago.EstadoPago } : null,
                    Detalles = "Cita reservada con éxito - Versión 2.0"
                };

                return StatusCode(201, new ApiResponse<object>(
                    respuestaMejorada,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.success.ToString(), Description = "Cita reservada correctamente." },
                        new Message { Type = TypeMessage.information.ToString(), Description = "API Versión 2.0" }
                    }
                ));
            }
            catch (BusinessException bex)
            {
                return StatusCode(bex.StatusCode, new ApiResponse<object>(
                    null,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.error.ToString(), Description = bex.Message }
                    }
                ));
            }
        }

        [HttpGet("listar")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Listar([FromQuery] CitaQueryFilter filters)
        {
            var result = await _service.GetAllCitasResponseAsync(filters);

            var citas = result.Pagination?.Cast<Cita>().ToList() ?? new List<Cita>();
            var estadisticas = new
            {
                TotalReservadas = citas.Count(c => c.Estado == "Reservada"),
                TotalCompletadas = citas.Count(c => c.Estado == "Completada"),
                TotalCanceladas = citas.Count(c => c.Estado == "Cancelada"),
                PromedioCitasPorDia = citas.GroupBy(c => c.FechaCita.Date).Average(g => g.Count())
            };

            var pagination = result.Pagination != null ? new Pagination
            {
                TotalCount = result.Pagination.TotalCount,
                PageSize = result.Pagination.PageSize,
                CurrentPage = result.Pagination.CurrentPage,
                TotalPages = result.Pagination.TotalPages,
                HasNextPage = result.Pagination.HasNextPage,
                HasPreviousPage = result.Pagination.HasPreviousPage
            } : null;

            var responseData = new
            {
                Citas = citas,
                Estadisticas = estadisticas,
                Metadata = new
                {
                    Version = "2.0",
                    Timestamp = DateTime.UtcNow,
                    Features = new[] { "Filtros avanzados", "Estadísticas", "Mejor respuesta" }
                }
            };

            var response = new ApiResponse<object>(
                responseData,
                pagination!,
                result.Messages?.ToList() ?? new List<Message>()
            );

            return StatusCode((int)result.StatusCode, response);
        }

        [HttpGet("dto/mapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Cita>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCitasDtoMapper([FromQuery] CitaQueryFilter filters)
        {
            try
            {
                var result = await _service.GetAllCitasResponseAsync(filters);

                var citas = result.Pagination?.Cast<Cita>().ToList() ?? new List<Cita>();

                var pagination = result.Pagination != null ? new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                } : null;

                var response = new ApiResponse<IEnumerable<Cita>>(citas)
                {
                    Pagination = pagination,
                    Messages = result.Messages?.ToList() ?? new List<Message>()
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception err)
            {
                return StatusCode(500, new ApiResponse<object>(
                    null,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.error.ToString(), Description = err.Message }
                    }
                ));
            }
        }

        [HttpPut("cancelar/{id}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Summary = "Cancela una cita médica - Versión 2.0", Description = "Versión mejorada con información detallada sobre tiempos y reembolsos")]
        public async Task<IActionResult> Cancelar(int id, [FromBody] CancelarCitaDto? dto = null)
        {
            if (dto != null)
            {
                var validation = await _cancelarValidator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    var errores = validation.Errors.Select(e => e.ErrorMessage).ToList();
                    return BadRequest(new ApiResponse<object>(
                        new { errores },
                        new List<Message>
                        {
                            new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validación - Versión 2.0" }
                        }
                    ));
                }
            }

            try
            {
                var cita = await _service.CancelarCitaAsync(id, dto?.MotivoCancelacion);

                var respuestaMejorada = new
                {
                    cita.Id,
                    cita.FechaCita,
                    cita.HoraCita,
                    cita.Estado,
                    Motivo = dto?.MotivoCancelacion ?? "Sin motivo especificado",
                    TiempoCancelacion = DateTime.Now,
                    HorasDeAnticipacion = (cita.FechaCita.Date.Add(cita.HoraCita) - DateTime.Now).TotalHours,
                    EstadoPago = cita.Pago?.EstadoPago,
                    Reembolso = cita.Pago?.EstadoPago == "Reembolsado" ? cita.Pago?.Monto : 0,
                    Detalles = "Cancelación procesada - Versión 2.0"
                };

                return Ok(new ApiResponse<object>(
                    respuestaMejorada,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.success.ToString(), Description = "Cita cancelada exitosamente." },
                        new Message { Type = TypeMessage.information.ToString(), Description = $"API Versión 2.0 - Tiempo restante: {respuestaMejorada.HorasDeAnticipacion:F1} horas" }
                    }
                ));
            }
            catch (BusinessException bex)
            {
                return StatusCode(bex.StatusCode, new ApiResponse<object>(
                    null,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.error.ToString(), Description = $"{bex.Message} - Versión 2.0" }
                    }
                ));
            }
        }
    }
}