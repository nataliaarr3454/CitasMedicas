using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;
        private readonly ILogger<SecurityService> _logger;

        public SecurityService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IPasswordService passwordService,
            ILogger<SecurityService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _passwordService = passwordService;
            _logger = logger;
        }

        public async Task<Security?> GetLoginByCredentials(UserLogin userLogin)
        {
            try
            {
                var user = await _unitOfWork.SecurityRepository.GetLoginByCredentials(userLogin);

                if (user == null)
                {
                    _logger.LogWarning("Usuario no encontrado: {Login}", userLogin.Login);
                    return null;
                }

                if (!_passwordService.Check(user.Password, userLogin.Password))
                {
                    _logger.LogWarning("Contraseña incorrecta para usuario: {Login}", userLogin.Login);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar credenciales para: {Login}", userLogin.Login);
                return null;
            }
        }

        public async Task<SecurityDto?> RegisterUser(SecurityDto securityDto)
        {
            var users = await _unitOfWork.SecurityRepository.GetAll();
            if (users.Any(u => u.Login == securityDto.Login))
            {
                _logger.LogWarning("Intento de registro con login duplicado: {Login}", securityDto.Login);
                return null;
            }

            var security = _mapper.Map<Security>(securityDto);

            security.Password = _passwordService.Hash(security.Password);

            await _unitOfWork.SecurityRepository.Add(security);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Usuario registrado exitosamente: {Login}", security.Login);
            return _mapper.Map<SecurityDto>(security);
        }

        public async Task<IEnumerable<SecurityDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.SecurityRepository.GetAll();

            var userDtos = _mapper.Map<IEnumerable<SecurityDto>>(users);
            foreach (var user in userDtos)
            {
                user.Password = "********"; 
            }

            return userDtos;
        }
    }
}