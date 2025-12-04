using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Core.Enums;
using CitasMedicas.Api.Responses;
using CitasMedicas.Core.CustomEntities;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace CitasMedicas.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISecurityService _securityService;
        private readonly IValidator<UserLogin> _validator;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<TokenController> _logger;

        public TokenController(
            IConfiguration configuration,
            ISecurityService securityService,
            IValidator<UserLogin> validator,
            IPasswordService passwordService,
            ILogger<TokenController> logger)
        {
            _configuration = configuration;
            _securityService = securityService;
            _validator = validator;
            _passwordService = passwordService;
            _logger = logger;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Authentication([FromBody] UserLogin userLogin)
        {
            _logger.LogInformation("Solicitud de autenticacion para usuario: {Login}", userLogin.Login);

            var validation = await _validator.ValidateAsync(userLogin);
            if (!validation.IsValid)
            {
                var errores = validation.Errors.Select(e => e.ErrorMessage).ToList();
                _logger.LogWarning("Validacion fallida para usuario: {Login}. Errores: {Errores}",
                    userLogin.Login, string.Join(", ", errores));
                return BadRequest(new ApiResponse<object>(
                    new { errores },
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = "Errores de validacion." } }
                ));
            }

            var user = await _securityService.GetLoginByCredentials(userLogin);
            if (user == null)
            {
                _logger.LogWarning("Autenticacion fallida para usuario: {Login}", userLogin.Login);
                return Unauthorized(new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = "Credenciales invalidas." } }
                ));
            }

            try
            {
                var token = GenerateToken(user);
                _logger.LogInformation("Token generado exitosamente para usuario: {Login}", user.Login);

                var response = new
                {
                    Token = token,
                    ExpiresIn = 1800, 
                    TokenType = "Bearer",
                    User = new
                    {
                        user.Id,
                        user.Name,
                        user.Login,
                        user.Role
                    }
                };

                return Ok(new ApiResponse<object>(
                    response,
                    new[] { new Message { Type = TypeMessage.success.ToString(), Description = "Token generado exitosamente." } }
                ));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token para usuario: {Login}", user.Login);
                return StatusCode(500, new ApiResponse<string>(
                    default!,
                    new[] { new Message { Type = TypeMessage.error.ToString(), Description = $"Error al generar token: {ex.Message}" } }
                ));
            }
        }

        private string GenerateToken(Security security)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Authentication:SecretKey"]!)
            );
            var signingCredentials = new SigningCredentials(
                symmetricSecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, security.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, security.Name),
                new Claim(ClaimTypes.Role, security.Role.ToString()),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                Issuer = _configuration["Authentication:Issuer"],
                Audience = _configuration["Authentication:Audience"],
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        [HttpGet("TestConexion")]
        [AllowAnonymous]
        public async Task<IActionResult> TestConexion()
        {
            try
            {
                var testPassword = "TestPassword123";
                var hashedPassword = _passwordService.Hash(testPassword);
                var checkResult = _passwordService.Check(hashedPassword, testPassword);

                var result = new
                {
                    Ambiente = _configuration["ASPNETCORE_ENVIRONMENT"] ?? "No configurado",
                    DatabaseProvider = _configuration["DatabaseProvider"] ?? "No configurado",
                    PasswordHashingTest = new
                    {
                        Original = testPassword,
                        Hashed = hashedPassword.Substring(0, 20) + "...", 
                        CheckResult = checkResult ? "OK" : "FAILED",
                        HashLength = hashedPassword.Length
                    },
                    PasswordOptions = new
                    {
                        SaltSize = _configuration["PasswordOptions:SaltSize"] ?? "No configurado",
                        Iterations = _configuration["PasswordOptions:Iterations"] ?? "No configurado"
                    }
                };

                return Ok(new ApiResponse<object>(
                    result,
                    new[] { new Message { Type = TypeMessage.information.ToString(), Description = "Test de conexion con hashing" } }
                ));
            }
            catch (Exception err)
            {
                return StatusCode(500,
                    new ApiResponse<string>(
                        default!,
                        new[] { new Message { Type = TypeMessage.error.ToString(), Description = $"Error: {err.Message}" } }
                    ));
            }
        }

        [HttpGet("HashExample/{password}")]
        [AllowAnonymous]
        public IActionResult HashExample(string password)
        {
            try
            {
                var hashed = _passwordService.Hash(password);
                return Ok(new
                {
                    Original = password,
                    Hashed = hashed,
                    Parts = hashed.Split('.'),
                    Length = hashed.Length
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }
    }
}