using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Core.Interfaces
{
    public interface ISecurityService
    {
        Task<Security?> GetLoginByCredentials(UserLogin userLogin);
        Task<SecurityDto?> RegisterUser(SecurityDto securityDto);
        Task<IEnumerable<SecurityDto>> GetAllUsersAsync();
    }
}