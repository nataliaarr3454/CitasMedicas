using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class PagoRepository : BaseRepository<Pago>, IPagoRepository
    {
        public PagoRepository(CitasMedicasContext context) : base(context)
        {
        }

        public async Task<Pago?> GetByCitaIdAsync(int citaId)
        {
            return await _entities
                .FirstOrDefaultAsync(p => p.CitaId == citaId);
        }

        public async Task<IEnumerable<Pago>> GetPagosPendientesAsync()
        {
            return await _entities
                .Where(p => p.EstadoPago == "Pendiente")
                .ToListAsync();
        }
    }
}