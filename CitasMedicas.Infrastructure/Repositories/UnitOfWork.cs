using CitasMedicas.Core.Entities;
using CitasMedicas.Core.Interfaces;
using CitasMedicas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Threading.Tasks;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CitasMedicasContext _context;
        private IDbContextTransaction? _efTransaction;
        private readonly IDapperContext? _dapper;

        private IBaseRepository<Medico>? _medicoRepository;
        private IBaseRepository<Paciente>? _pacienteRepository;
        private IBaseRepository<Disponibilidad>? _disponibilidadRepository;
        private IBaseRepository<Cita>? _citaRepository;
        private IBaseRepository<Pago>? _pagoRepository;

        public UnitOfWork(CitasMedicasContext context, IDapperContext? dapper = null)
        {
            _context = context;
            _dapper = dapper;
        }

        public IBaseRepository<Medico> MedicoRepository =>
            _medicoRepository ??= new BaseRepository<Medico>(_context);

        public IBaseRepository<Paciente> PacienteRepository =>
            _pacienteRepository ??= new BaseRepository<Paciente>(_context);

        public IBaseRepository<Disponibilidad> DisponibilidadRepository =>
            _disponibilidadRepository ??= new BaseRepository<Disponibilidad>(_context);

        public IBaseRepository<Cita> CitaRepository =>
            _citaRepository ??= new BaseRepository<Cita>(_context);

        public IBaseRepository<Pago> PagoRepository =>
            _pagoRepository ??= new BaseRepository<Pago>(_context);

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        #region Transacciones
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
        #endregion

        public void Dispose()
        {
            _efTransaction?.Dispose();
            _context.Dispose();
        }
    }
}
