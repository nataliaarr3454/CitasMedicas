using CitasMedicas.Core.DTOs;
using System.Threading.Tasks;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Core.Interfaces
{
    public interface ISecurityService
    {
        Task<Security> GetLoginByCredentials(UserLogin userLogin);
        Task RegisterUser(Security security);
    }
}