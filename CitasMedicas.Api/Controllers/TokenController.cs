using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CitasMedicas.Api.Controllers
{
    /// <summary>
    /// Controlador responsable de la autenticacion y generacion de tokens JWT.
    /// </summary>
    /// <remarks>
    /// Este controlador permite a los usuarios autenticarse y obtener un token JWT
    /// que sera utilizado para acceder a los endpoints protegidos de la API.
    /// 
    /// El token incluye informacion del usuario y su rol.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        
        public TokenController(
            IConfiguration configuration,
            ISecurityService securityService)
        {
            _configuration = configuration;
            _securityService = securityService;
        }

        /// <summary>
        /// Autentica un usuario y genera un token JWT.
        /// </summary>
        /// <remarks>
        /// Este endpoint valida las credenciales del usuario
        /// devuelve un token JWT valido por un tiempo determinado.
        /// 
        /// El token incluye:
        /// - Login, Nombre, Rol del usuario
        /// 
        /// El token debe enviarse en el header:
        /// Authorization: Bearer {token}
        /// </remarks>
        /// <param name="userLogin">Credenciales del usuario.</param>
        /// <returns>Token JWT generado.</returns>
        
        [AllowAnonymous]
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        [SwaggerOperation(
            Summary = "Login de usuario",
            Description = "Autentica un usuario y devuelve un token JWT."
        )]
        public async Task<IActionResult> Authentication(UserLogin userLogin)
        {
            var user = await _securityService.GetLoginByCredentials(userLogin);

            if (user == null)
                return NotFound("Credenciales incorrectas");

            var token = GenerateToken(user);
            return Ok(new { token });
        }
        
        
        /// <summary>
        /// Genera el token JWT a partir de los datos del usuario.
        /// </summary>
        private string GenerateToken(Security security)
        {

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"])
            );

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256
            );

            var header = new JwtHeader(credentials);

            var claims = new[]
            {
            new Claim("Login", security.Login),
            new Claim("Name", security.Name),
            new Claim(ClaimTypes.Role, security.Role.ToString())
        };


            var payload = new JwtPayload(
                issuer: _configuration["Authentication:Issuer"],
                audience: _configuration["Authentication:Audience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(15)
            );

            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("TestConeccion")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConeccion()
        {
            var result = new
            {
                ConnectionMySql = _configuration["ConnectionStrings:ConnectionMySql"],
                ConnectionSqlServer = _configuration["ConnectionStrings:ConnectionSqlServer"]
            };

            return Ok(result);
        }



        [HttpGet("TestConeccionDetallado")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConeccionDetallado()
        {
            try
            {
                var connectionString = _configuration["ConnectionStrings:ConnectionSqlServer"];

                // Limpiar y mostrar la cadena real
                var cleanedConnectionString = connectionString?.Replace("\\\\\\\\", "\\").Replace("\\\\", "\\");

                var result = new
                {
                    CadenaOriginal = connectionString,
                    CadenaLimpia = cleanedConnectionString,
                    LongitudOriginal = connectionString?.Length,
                    LongitudLimpia = cleanedConnectionString?.Length,
                    ProblemaDetectado = connectionString?.Contains("\\\\\\\\") == true ? "Demasiadas barras invertidas" : "OK"
                };

                // Intentar conexión SIN Dapper
                using (var connection = new SqlConnection(cleanedConnectionString))
                {
                    await connection.OpenAsync();

                    string versionSQL = "";
                    string usuario = "";

                    // Consultar versión SQL
                    using (var cmd = new SqlCommand("SELECT @@VERSION", connection))
                    {
                        versionSQL = (await cmd.ExecuteScalarAsync())?.ToString() ?? "";
                    }

                    // Consultar usuario
                    using (var cmd = new SqlCommand("SELECT SUSER_NAME()", connection))
                    {
                        usuario = (await cmd.ExecuteScalarAsync())?.ToString() ?? "";
                    }

                    var serverInfo = new
                    {
                        Estado = "Conexión exitosa",
                        Servidor = connection.DataSource,
                        BaseDeDatos = connection.Database,
                        VersionSQL = versionSQL,
                        Usuario = usuario,
                        EstadoConexion = connection.State.ToString()
                    };

                    return Ok(new
                    {
                        Diagnostico = result,
                        Conexion = serverInfo
                    });
                }
            }
            catch (SqlException sqlEx)
            {
                return BadRequest(new
                {
                    Error = "Error de SQL Server",
                    NumeroError = sqlEx.Number,
                    Mensaje = sqlEx.Message,
                    SolucionSugerida = GetSqlErrorSolution(sqlEx.Number),
                    Instrucciones = new[] {
                "1. Verifica que SQL Server Express está ejecutándose",
                "2. Abre SQL Server Configuration Manager",
                "3. Habilita TCP/IP en Protocols for SQLEXPRESS",
                "4. Reinicia el servicio SQL Server (SQLEXPRESS)",
                "5. Prueba esta cadena en SSMS: Server=DESKTOP-Q6F6H02\\SQLEXPRESS;Trusted_Connection=True;"
            }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Error = " Error general",
                    Mensaje = ex.Message,
                    Tipo = ex.GetType().Name,
                    StackTrace = ex.StackTrace
                });
            }
        }

        private string GetSqlErrorSolution(int errorNumber)
        {
            return errorNumber switch
            {
                53 => "Error 53: No se puede encontrar el servidor. Verifica que el nombre 'DESKTOP-Q6F6H02\\SQLEXPRESS' es correcto.",
                4060 => "Error 4060: La base de datos 'DBCitasMedicasOnline' no existe. Ejecuta el script SQL para crearla.",
                18456 => "Error 18456: Error de autenticación. Usa Windows Authentication.",
                40 => "Error 40: No se puede abrir la conexión. SQL Server no está disponible.",
                26 => "Error 26: Error al localizar el servidor/instancia especificada.",
                _ => $"Error SQL {errorNumber}. Consulta los logs de SQL Server."
            };
        }



    }


}

