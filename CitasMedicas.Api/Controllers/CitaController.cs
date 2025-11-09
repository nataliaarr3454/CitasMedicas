using CitasMedicas.Api.Responses;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CitasMedicas.Api.Controllers
{
    /// <summary>
    /// Controlador que gestiona las operaciones relacionadas con las citas médicas.
    /// </summary>
    /// <remarks>
    /// Permite reservar citas, listar citas con paginación y filtrar según diferentes criterios.
    /// </remarks>
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _service;
        private readonly IValidator<ReservaCitaDto> _validator;

        public CitaController(ICitaService service, IValidator<ReservaCitaDto> validator)
        {
            _service = service;
            _validator = validator;
        }

        /// <summary>
        /// Registra una nueva cita médica en el sistema.
        /// </summary>
        /// <remarks>
        /// Este método permite a un paciente reservar una cita médica con un médico disponible.  
        /// Se valida la disponibilidad y los datos ingresados antes de realizar el registro.  
        /// Retorna un objeto <see cref="ApiResponse{Cita}"/> con los detalles de la cita creada.
        /// </remarks>
        /// <param name="dto">Datos de la cita que se desea reservar.</param>
        /// <returns>Una respuesta con el detalle de la cita registrada o un mensaje de error.</returns>
        /// <response code="201">Cita registrada correctamente.</response>
        /// <response code="400">Error de validación o cita no disponible.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpPost("reservar")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<Cita>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ApiResponse<string>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse<string>))]
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
                return StatusCode(201, new ApiResponse<Cita>(
                    cita,
                    new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Cita reservada correctamente." } }
                ));
            }
            catch (BusinessException bex)
            {
                return StatusCode(bex.StatusCode, new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = bex.Message } }
                ));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = $"Error inesperado: {ex.Message}" } }
                ));
            }
        }

        /// <summary>
        /// Recupera una lista paginada de citas médicas según los filtros especificados.
        /// </summary>
        /// <remarks>
        /// Este método devuelve una colección paginada de citas con la información de paginación incluida en la respuesta.
        /// Permite filtrar por paciente, médico, fecha o estado.
        /// </remarks>
        /// <param name="filters">Filtros de búsqueda y paginación.</param>
        /// <returns>Una lista paginada de citas.</returns>
        /// <response code="200">Retorna la lista de citas.</response>
        /// <response code="500">Error interno del servidor.</response>
        [HttpGet("listar")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Cita>>))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError, Type = typeof(ApiResponse<string>))]
        public async Task<IActionResult> Listar([FromQuery] CitaQueryFilter filters)
        {
            var result = await _service.GetAllCitasResponseAsync(filters);

            var pagination = result.Pagination != null ? new Pagination
            {
                TotalCount = result.Pagination.TotalCount,
                PageSize = result.Pagination.PageSize,
                CurrentPage = result.Pagination.CurrentPage,
                TotalPages = result.Pagination.TotalPages,
                HasNextPage = result.Pagination.HasNextPage,
                HasPreviousPage = result.Pagination.HasPreviousPage
            } : null;

            var response = new ApiResponse<object>(
                result.Pagination,
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
