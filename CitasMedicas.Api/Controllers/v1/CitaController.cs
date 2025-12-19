using AutoMapper;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Exceptions;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.QueryFilters;
using CitasMedicas.Infrastructure.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Linq;

namespace CitasMedicas.Api.Controllers.v1
{
    [Authorize(Roles = $"{nameof(RoleType.Administrador)},{nameof(RoleType.Medico)},{nameof(RoleType.Paciente)}")]
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CitaController : ControllerBase
    {
        private readonly ICitaService _service;
        private readonly IValidator<ReservaCitaDto> _validator;
        private readonly IDapperContext _dapper;
        private readonly IMapper _mapper;

        public CitaController(
            ICitaService service,
            IDapperContext dapper,
            IMapper mapper,
            IValidator<ReservaCitaDto> validator)
        {
            _service = service;
            _dapper = dapper;
            _mapper = mapper;
            _validator = validator;
        }

        [HttpPost("reservar")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [SwaggerOperation(Summary = "Registrar cita", Description = "Registra una cita ocupando una disponibilidad.")]
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
                return StatusCode(201, new ApiResponse<Cita>(
                    cita,
                    new List<Message>
                    {
                        new Message { Type = TypeMessage.success.ToString(), Description = "Cita reservada correctamente." }
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
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<IEnumerable<Cita>>))]
        [SwaggerOperation(Summary = "Filtrar citas médicas", Description = "Obtiene un listado paginado de citas con filtros.")]
        public async Task<IActionResult> Listar([FromQuery] CitaQueryFilter filters)
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
    }
}