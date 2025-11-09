using System;
using System.Data;
using System.Threading.Tasks;
using CitasMedicas.Core.Entities;

namespace CitasMedicas.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBaseRepository<Medico> MedicoRepository { get; }
        IBaseRepository<Paciente> PacienteRepository { get; }
        IBaseRepository<Disponibilidad> DisponibilidadRepository { get; }
        IBaseRepository<Cita> CitaRepository { get; }
        IBaseRepository<Pago> PagoRepository { get; }

        void SaveChanges();
        Task SaveChangesAsync();

        // --- Manejo de transacciones EF ---
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();

        // --- Soporte para Dapper ---
        IDbConnection? GetDbConnection();
        IDbTransaction? GetDbTransaction();
    }
}
