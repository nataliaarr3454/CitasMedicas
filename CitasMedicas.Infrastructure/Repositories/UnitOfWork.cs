using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CitasMedicasContext _context;
        private IDbContextTransaction _efTransaction;
        private readonly IDapperContext _dapper;

        private readonly IMedicoRepository _medico;
        private readonly IPacienteRepository _paciente;
        private readonly IDisponibilidadRepository _disponibilidad;
        private readonly ICitaRepository _cita;
        private readonly IPagoRepository _pago;
        private readonly ISecurityRepository _security;

        public UnitOfWork(CitasMedicasContext context, IDapperContext dapper)
        {
            _context = context;
            _dapper = dapper;

            _medico = new MedicoRepository(context);
            _paciente = new PacienteRepository(context);
            _disponibilidad = new DisponibilidadRepository(context);
            _cita = new CitaRepository(context);
            _pago = new PagoRepository(context);
            _security = new SecurityRepository(context);
        }

        public IMedicoRepository Medico => _medico;
        public IPacienteRepository Paciente => _paciente;
        public IDisponibilidadRepository Disponibilidad => _disponibilidad;
        public ICitaRepository Cita => _cita;
        public IPagoRepository Pago => _pago;
        public ISecurityRepository SecurityRepository => _security;
        public IDapperContext Dapper => _dapper;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task BeginTransactionAsync()
        {
            if (_efTransaction == null)
            {
                _efTransaction = await _context.Database.BeginTransactionAsync();
                var conn = _context.Database.GetDbConnection();
                var tx = _efTransaction.GetDbTransaction();
                _dapper?.SetAmbientConnection(conn, tx);
            }
        }

        public async Task CommitAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_efTransaction != null)
                {
                    await _efTransaction.CommitAsync();
                    await _efTransaction.DisposeAsync();
                    _efTransaction = null;
                }
            }
            finally
            {
                _dapper?.ClearAmbientConnection();
            }
        }

        public async Task RollbackAsync()
        {
            if (_efTransaction != null)
            {
                await _efTransaction.RollbackAsync();
                await _efTransaction.DisposeAsync();
                _efTransaction = null;
            }
            _dapper?.ClearAmbientConnection();
        }

        public IDbConnection? GetDbConnection() => _context.Database.GetDbConnection();
        public IDbTransaction? GetDbTransaction() => _efTransaction?.GetDbTransaction();

        public void Dispose()
        {
            _efTransaction?.Dispose();
            _context.Dispose();
        }
    }
}