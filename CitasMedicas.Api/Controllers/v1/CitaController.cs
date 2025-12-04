using CitasMedicas.Api.Responses;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CitasMedicas.Api.Controllers.v1
{
    /// <summary>
    /// Controlador que gestiona las operaciones relacionadas con las citas médicas (v1).
    /// </summary>
    /// <remarks>
    /// Permite reservar citas, listar citas con paginación y filtrar según diferentes criterios.
    /// </remarks>
    [Authorize(Roles = $"{nameof(RoleType.Administrador)},{nameof(RoleType.Medico)},{nameof(RoleType.Paciente)}")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// Registra una nueva cita médica en el sistema (v1).
        /// </summary>
        [HttpPost("reservar")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(ApiResponse<Cita>))]
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
        }

        /// <summary>
        /// Recupera una lista paginada de citas médicas según los filtros especificados (v1).
        /// </summary>
        [HttpGet("listar")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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
    }
}