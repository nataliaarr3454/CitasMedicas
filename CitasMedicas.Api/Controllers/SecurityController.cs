using AutoMapper;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CitasMedicas.Api.Controllers
{
    /// <summary>
    /// Controlador encargado de la gestion de usuarios del sistema.
    /// medico,paciente y administrados
    /// </summary>
    /// <remarks>
    /// Permite registrar usuarios en la tabla de seguridad.
    /// 
    /// Acceso restringido:
    /// - Solo usuarios con rol <b>Administrador</b> pueden acceder a este controlador.
    /// 
    /// </remarks>
    //[Authorize(Roles = $"{nameof(RoleType.Administrador)}")]
    //lo bloquie para poder crear un admi
    [AllowAnonymous] //borrar esto
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public SecurityController(ISecurityService securityService, IMapper mapper, IPasswordService passwordService)
        {
            _securityService = securityService;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        /// <summary>
        /// Registra un nuevo usuario del sistema.
        /// </summary>
        /// <remarks>
        /// Este endpoint permite crear usuarios que podran autenticarse en la API.
        /// 
        /// Reglas:
        /// - Solo el rol <b>Administrador</b> puede registrar usuarios.
        /// - La contraseña se almacena de forma segura utilizando hash.
        /// - El rol define los permisos del usuario.
        /// 
        /// Caso de uso:
        /// Registro de usuarios administrativos, veterinarios o recepcionistas.
        /// </remarks>
        /// <param name="securityDto">Datos del usuario a registrar.</param>
        /// <returns>Informacion del usuario registrado.</returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ApiResponse<SecurityDto>))]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
            Summary = "Registrar usuario",
            Description = "Permite registrar un usuario en el sistema (solo Administrador)."
        )]
        public async Task<IActionResult> Post(SecurityDto securityDto)
        {
            var security = _mapper.Map<Security>(securityDto);

            // Generar HASH antes de guardar
            security.Password = _passwordService.Hash(security.Password);

            await _securityService.RegisterUser(security);

            securityDto = _mapper.Map<SecurityDto>(security);
            var response = new ApiResponse<SecurityDto>(securityDto);

            return Ok(response);
        }
    }
}
