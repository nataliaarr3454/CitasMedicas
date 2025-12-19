using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class PacienteRepository : BaseRepository<Paciente>, IPacienteRepository
    {
        public PacienteRepository(CitasMedicasContext context) : base(context)
        {
        }

        public async Task<Paciente?> GetByCorreoAsync(string correo)
        {
            return await _entities
                .FirstOrDefaultAsync(p => p.Correo == correo);
        }

        public async Task<Paciente?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
    }
}