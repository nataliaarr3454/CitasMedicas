using CitasMedicas.Core.Entities;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IPacienteRepository : IBaseRepository<Paciente>
    {
        Task<Paciente?> GetByCorreoAsync(string correo);
        Task<Paciente?> GetByIdAsync(int id);
    }
}