using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class SecurityRepository : BaseRepository<Security>, ISecurityRepository
    {
        private readonly CitasMedicasContext _context;
        private readonly DbSet<Security> _entities;

        public SecurityRepository(CitasMedicasContext context) : base(context)
        {
            _context = context;
            _entities = context.Set<Security>();
        }

        public async Task<Security?> GetLoginByCredentials(UserLogin login)
        {
            return await _entities
                    .FirstOrDefaultAsync(x => x.Login == login.Login);
        }
    }
}