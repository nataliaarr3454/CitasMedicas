using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class DisponibilidadRepository : BaseRepository<Disponibilidad>, IDisponibilidadRepository
    {
        public DisponibilidadRepository(CitasMedicasContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Disponibilidad>> GetByMedicoAsync(int medicoId)
        {
            return await _entities
                .Where(d => d.MedicoId == medicoId)
                .ToListAsync();
        }

        public async Task<bool> ExisteSolapamientoAsync(int medicoId, Disponibilidad nuevaDisponibilidad)
        {
            return await _entities.AnyAsync(d =>
                d.MedicoId == medicoId &&
                d.Fecha.Date == nuevaDisponibilidad.Fecha.Date &&
                ((d.HoraInicio < nuevaDisponibilidad.HoraFin && d.HoraFin > nuevaDisponibilidad.HoraInicio) ||
                 (nuevaDisponibilidad.HoraInicio < d.HoraFin && nuevaDisponibilidad.HoraFin > d.HoraInicio))
            );
        }

        public async Task<Disponibilidad?> GetByIdAsync(int id)
        {
            return await _entities.FindAsync(id);
        }
    }
}