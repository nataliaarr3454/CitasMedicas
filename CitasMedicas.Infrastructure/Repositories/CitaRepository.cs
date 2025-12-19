using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class CitaRepository : BaseRepository<Cita>, ICitaRepository
    {
        public CitaRepository(CitasMedicasContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Cita>> GetByPacienteIdAsync(int pacienteId)
        {
            return await _entities
                .Where(c => c.PacienteId == pacienteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Cita>> GetByMedicoIdAsync(int medicoId)
        {
            return await _entities
                .Where(c => c.MedicoId == medicoId)
                .ToListAsync();
        }
    }
}