using CitasMedicas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IPagoRepository : IBaseRepository<Pago>
    {
        Task<Pago?> GetByCitaIdAsync(int citaId);
        Task<IEnumerable<Pago>> GetPagosPendientesAsync();
    }
}