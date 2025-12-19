using AutoMapper;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Services
{
    public class SecurityService : ISecurityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;

        public SecurityService(IUnitOfWork unitOfWork, IPasswordService passwordService)
        {
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
        }

        public async Task<Security?> GetLoginByCredentials(UserLogin userLogin)
        {
            var user = await _unitOfWork.SecurityRepository
                .GetLoginByCredentials(userLogin);

            if (user == null)
                return null;

            var valid = _passwordService.Check(user.Password, userLogin.Password);

            return valid ? user : null;
        }

        public async Task RegisterUser(Security security)
        {
            _unitOfWork.SecurityRepository.Add(security);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}