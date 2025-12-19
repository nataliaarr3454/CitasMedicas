using System.Collections.Generic;
using System.Threading.Tasks;
using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using CitasMedicas.Core.QueryFilters;

namespace CitasMedicas.Core.Interfaces
{
    public interface IMedicoService
    {
        Task<MedicoDto?> RegistrarMedicoAsync(MedicoDto dto);
        Task<IEnumerable<MedicoDto>> ObtenerMedicosAsync();
        Task<ResponseData>Obtenermedicos(MedicoQueryFilter filter);
        Task UpdateMedico(Medico medico);
        Task DeleteMedico(int medico);


        Task<IEnumerable<MedicoCantidadCitasDto>> GetCantidadCitasPorMedicoAsync();
    }
}
