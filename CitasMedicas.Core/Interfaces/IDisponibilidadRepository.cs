using CitasMedicas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IDisponibilidadRepository : IBaseRepository<Disponibilidad>
    {
        Task<IEnumerable<Disponibilidad>> GetByMedicoAsync(int medicoId);
        Task<bool> ExisteSolapamientoAsync(int medicoId, Disponibilidad nuevaDisponibilidad);
        Task<Disponibilidad?> GetByIdAsync(int id);
    }
}