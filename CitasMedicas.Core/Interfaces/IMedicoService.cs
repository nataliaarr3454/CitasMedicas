using System.Collections.Generic;
using System.Threading.Tasks;
using CitasMedicas.Core.DTOs;

namespace CitasMedicas.Core.Interfaces
{
    public interface IMedicoService
    {
        Task<MedicoDto?> RegistrarMedicoAsync(MedicoDto dto);
        Task<IEnumerable<MedicoDto>> ObtenerMedicosAsync();
    }
}
