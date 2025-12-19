using CitasMedicas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.QueryFilters;

namespace CitasMedicas.Core.Interfaces
{
    public interface IDisponibilidadService
    {
        Task<Disponibilidad?> RegistrarDisponibilidadAsync(Disponibilidad disponibilidad);

        Task<IEnumerable<Disponibilidad>> ObtenerDisponibilidadesPorMedicoAsync(int medicoId);
        Task<ResponseData> ObtenerDisponibilidadesFiltradasAsync(DisponibilidadQueryFilter filters);
        Task UpdateDisponibilidad(Disponibilidad disponibilidad);

    }
}
