using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CitasMedicas.Api.Controllers.v1
{
    //[Authorize(Roles = $"{nameof(RoleType.Administrador)}")]
    //lo bloquie para poder crear un admi
    [AllowAnonymous] //borrar esto
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly ISecurityService _securityService;
        private readonly IValidator<SecurityDto> _validator;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            ISecurityService securityService,
            IValidator<SecurityDto> validator,
            ILogger<SecurityController> logger)
        {
            _securityService = securityService;
            _validator = validator;
            _logger = logger;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> Registrar([FromBody] SecurityDto dto)
        {
            _logger.LogInformation("Solicitud de registro de usuario: {Login}", dto.Login);

            var validation = await _validator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validacion fallida para registro: {Login}. Errores: {Errores}",
                    dto.Login, string.Join(", ", errores));
                return BadRequest(new ApiResponse<object>(
                    new { errores },
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validacion." } }
                ));
            }

            try
            {
                var usuario = await _securityService.RegisterUser(dto);

                if (usuario == null)
                {
                    _logger.LogWarning("Registro fallido: Login duplicado - {Login}", dto.Login);
                    return BadRequest(new ApiResponse<string>(
                        default!,
                        new[] { new Message { Type = TypeMessage.warning.ToString(), Description = "El login ya esta registrado." } }
                    ));
                }

                usuario.Password = "********";

                _logger.LogInformation("Usuario registrado exitosamente: {Login}", dto.Login);
                return StatusCode(201, new ApiResponse<SecurityDto>(
                    usuario,
                    new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Usuario registrado correctamente." } }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario: {Login}", dto.Login);
                return BadRequest(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = $"Error: {ex.Message}" } }
                ));
            }
        }

        [HttpGet("listar")]
        public async Task<IActionResult> Listar()
        {
            _logger.LogInformation("Solicitud de listado de usuarios");

            var lista = await _securityService.GetAllUsersAsync();
            return Ok(new ApiResponse<IEnumerable<SecurityDto>>(
                lista,
                new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Lista de usuarios obtenida correctamente." } }
            ));
        }

        [HttpGet("test-hash/{password}")]
        [AllowAnonymous]
        public IActionResult TestHash(string password)
        {
            var hashService = HttpContext.RequestServices.GetService<IPasswordService>();

            if (hashService == null)
                return BadRequest(new { Error = "PasswordService no disponible" });

            var hashed = hashService.Hash(password);
            var checkResult = hashService.Check(hashed, password);

            return Ok(new
            {
                Original = password,
                Hashed = hashed,
                CheckResult = checkResult,
                Parts = hashed.Split('.'),
                Analysis = new
                {
                    TotalLength = hashed.Length,
                    Part1 = hashed.Split('.')[0] + " (iteraciones)",
                    Part2Length = hashed.Split('.')[1].Length + " (salt)",
                    Part3Length = hashed.Split('.')[2].Length + " (hash)"
                }
            });
        }
    }
}