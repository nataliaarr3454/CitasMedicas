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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CitasMedicas.Api.Controllers.v2
{
    /// <summary>
    /// Controlador que gestiona las operaciones relacionadas con las citas médicas (v2).
    /// </summary>
    /// <remarks>
    /// Versión mejorada que incluye filtros avanzados y estadísticas.
    /// </remarks>
    [Authorize]
    [Produces("application/json")]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _service;
        private readonly IValidator<ReservaCitaDto> _validator;
        private readonly IMapper _mapper;

        public CitaController(ICitaService service, IValidator<ReservaCitaDto> validator, IMapper mapper)
        {
            _service = service;
            _validator = validator;
            _mapper = mapper;

        }

        /// <summary>
        /// Registra una nueva cita médica en el sistema (v2).
        /// </summary>
        /// <remarks>
        /// Versión mejorada con validaciones adicionales y mejor respuesta
        /// </remarks>
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
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validación." } }
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
                    new[] {
                        new Message { Type = TypeMessage.success.ToString(), Description = "Cita reservada correctamente." },
                        new Message { Type = TypeMessage.information.ToString(), Description = "API Versión 2.0" }
                    }
                ));
            }
            catch (BusinessException bex)
            {
                return StatusCode(bex.StatusCode, new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = bex.Message } }
                ));
            }
        }

        /// <summary>
        /// Recupera una lista paginada de citas médicas según los filtros especificados (v2).
        /// </summary>
        /// <remarks>
        /// Versión mejorada con filtros.
        /// </remarks>
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
                Citas = result.Pagination,
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
                result.Messages!
            );

            return StatusCode((int)result.StatusCode, response);
        }
        /// <summary>
        /// Recupera una lista paginada de citas como DTOs usando AutoMapper.
        /// </summary>
        /// <remarks>
        /// Convierte los objetos <see cref="Cita"/> en <see cref="ReservaCitaDto"/> utilizando AutoMapper.  
        /// Retorna los resultados junto con los datos de paginación.
        /// </remarks>
        /// <param name="filters">Filtros aplicables para la búsqueda y paginación.</param>
        /// <returns>Lista paginada de objetos <see cref="ReservaCitaDto"/>.</returns>
        /// <response code="200">Citas obtenidas correctamente.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("dto/mapper")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<ReservaCitaDto>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> GetCitasDtoMapper([FromQuery] CitaQueryFilter filters)
        {
            try
            {
                var result = await _service.GetAllCitasResponseAsync(filters);

                var citasDto = result.Pagination != null
                    ? result.Pagination.Cast<Cita>().ToList()
                    : new List<Cita>();

                var pagination = result.Pagination != null ? new Pagination
                {
                    TotalCount = result.Pagination.TotalCount,
                    PageSize = result.Pagination.PageSize,
                    CurrentPage = result.Pagination.CurrentPage,
                    TotalPages = result.Pagination.TotalPages,
                    HasNextPage = result.Pagination.HasNextPage,
                    HasPreviousPage = result.Pagination.HasPreviousPage
                } : null;

                var response = new ApiResponse<IEnumerable<Cita>>(citasDto)
                {
                    Pagination = pagination,
                    Messages = result.Messages
                };

                return StatusCode((int)result.StatusCode, response);
            }
            catch (Exception err)
            {
                var responseError = new ResponseData()
                {
                    Messages = new[]
                    {
                        new Message { Type = TypeMessage.error.ToString(), Description = err.Message }
                    },
                };
                return StatusCode(500, responseError);
            }
        }
    }
}