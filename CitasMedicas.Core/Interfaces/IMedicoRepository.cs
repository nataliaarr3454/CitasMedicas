using CitasMedicas.Core.Entities;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IMedicoRepository : IBaseRepository<Medico>
    {
        Task<Medico?> GetByCorreoAsync(string correo);
        Task<Medico?> GetByIdAsync(int id);
    }
}