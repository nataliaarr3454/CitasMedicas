using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Core.Interfaces
{
    public interface ISecurityRepository : IBaseRepository<Security>
    {
        Task<Security?> GetLoginByCredentials(UserLogin login);
    }
}