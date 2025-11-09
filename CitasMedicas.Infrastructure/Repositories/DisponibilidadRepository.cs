/*using Microsoft.EntityFrameworkCore;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class DisponibilidadRepository : IDisponibilidadRepository
    {
        private readonly CitasMedicasContext _context;

        public DisponibilidadRepository(CitasMedicasContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Disponibilidad disponibilidad)
        {
            _context.Disponibilidades.Add(disponibilidad);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Disponibilidad>> GetByMedicoAsync(int medicoId)
        {
            return await _context.Disponibilidades
                .Where(d => d.MedicoId == medicoId)
                .ToListAsync();
        }

        public async Task<bool> ExisteSolapamientoAsync(int medicoId, Disponibilidad nueva)
        {
            var disponibilidades = await _context.Disponibilidades
                .Where(d => d.MedicoId == medicoId)
                .ToListAsync();

            foreach (var d in disponibilidades)
            {
                if (nueva.Fecha == d.Fecha &&
                    ((nueva.HoraInicio < d.HoraFin) && (d.HoraInicio < nueva.HoraFin)))
                {
                    return true; 
                }
            }
            return false;
        }
        public async Task<Disponibilidad?> GetByIdAsync(int id)
        {
            return await _context.Disponibilidades.FirstOrDefaultAsync(d => d.Id == id);
        }


    }
}
*/
