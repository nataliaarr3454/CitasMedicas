using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
    {
        public SecurityRepository(CitasMedicasContext context) : base(context)
        {        }

        public async Task<Security?> GetLoginByCredentials(UserLogin login)
        {
            return await _entities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Login == login.Login);
        }
    }
}