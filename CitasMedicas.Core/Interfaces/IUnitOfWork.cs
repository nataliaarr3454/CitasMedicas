using CitasMedicas.Core.Entities;
using System;
using System.Data;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IMedicoRepository Medico { get; }
        IPacienteRepository Paciente { get; }
        IDisponibilidadRepository Disponibilidad { get; }
        ICitaRepository Cita { get; }
        IPagoRepository Pago { get; }
        ISecurityRepository SecurityRepository { get; }

        IDapperContext Dapper { get; }

        Task SaveChangesAsync();
        void SaveChanges();

        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();

    }
}
