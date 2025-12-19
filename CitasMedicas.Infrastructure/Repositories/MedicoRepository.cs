using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class MedicoRepository : BaseRepository<Medico>, IMedicoRepository
    {
        public MedicoRepository(CitasMedicasContext context) : base(context)
        {
        }

        public async Task<Medico?> GetByCorreoAsync(string correo)
        {
            return await _entities
                .FirstOrDefaultAsync(m => m.Correo == correo);
        }

        public async Task<Medico?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
    }
}