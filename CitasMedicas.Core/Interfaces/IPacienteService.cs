using CitasMedicas.Core.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IPacienteService
    {
        Task<PacienteDto?> RegistrarPacienteAsync(PacienteDto dto);
        Task<IEnumerable<PacienteDto>> ObtenerPacientesAsync();
    }
}
