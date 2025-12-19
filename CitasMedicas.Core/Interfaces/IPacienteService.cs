using CitasMedicas.Core.DTOs;
using CitasMedicas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IPacienteService
    {
        Task<PacienteDto?> RegistrarPacienteAsync(PacienteDto dto);
        Task<IEnumerable<PacienteDto>> ObtenerPacientesAsync();
        Task UpdatePaciente(Paciente paciente);
        Task DeletePaciente(int id);
    }
}
