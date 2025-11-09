using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;
using Dapper;

namespace CitasMedicas.Infrastructure.Data
{
    /// <summary>
    /// Contexto Dapper que permite ejecutar comandos SQL nativos.
    /// Soporta integración con UnitOfWork mediante conexión ambiente (AsyncLocal).
    /// </summary>
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnectionFactory _connFactory;

        private static readonly AsyncLocal<(IDbConnection? Conn, IDbTransaction? Tx)> _ambient = new();

        public DatabaseProvider Provider => _connFactory.Provider;

        public DapperContext(IDbConnectionFactory connFactory)
        {
            _connFactory = connFactory;
        }

        #region UnitOfWork integration
        public void SetAmbientConnection(IDbConnection conn, IDbTransaction? tx)
        {
            _ambient.Value = (conn, tx);
        }

        public void ClearAmbientConnection()
        {
            _ambient.Value = (null, null);
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Devuelve la conexión y transacción activas.
        /// Si no hay un UnitOfWork, crea una conexión nueva.
        /// </summary>
        private (IDbConnection conn, IDbTransaction? tx, bool ownsConnection) GetConnAndTx()
        {
            var ambient = _ambient.Value;
            if (ambient.Conn != null)
                return (ambient.Conn, ambient.Tx, false);

            var conn = _connFactory.CreateConnection();
            return (conn, null, true);
        }

        private async Task OpenIfNeededAsync(IDbConnection conn)
        {
            if (conn is DbConnection dbConn && dbConn.State == ConnectionState.Closed)
                await dbConn.OpenAsync();
        }

        private async Task CloseIfOwnedAsync(IDbConnection conn, bool owns)
        {
            if (!owns) return;

            if (conn is DbConnection dbConn && dbConn.State != ConnectionState.Closed)
                await dbConn.CloseAsync();

            conn.Dispose();
        }

        #endregion

        #region Query Methods

        /// <summary>
        /// Ejecuta una consulta SELECT que devuelve múltiples filas.
        /// </summary>
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryAsync<T>(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar QueryAsync: {ex.Message}");
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        /// <summary>
        /// Devuelve una única fila o null.
        /// </summary>
        public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar QueryFirstOrDefaultAsync: {ex.Message}");
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        /// <summary>
        /// Ejecuta un comando que no devuelve resultados (INSERT, UPDATE, DELETE).
        /// </summary>
        public async Task<int> ExecuteAsync(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeededAsync(conn);
                return await conn.ExecuteAsync(new CommandDefinition(sql, param, tx, commandType: commandType));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar ExecuteAsync: {ex.Message}");
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        /// <summary>
        /// Ejecuta un query escalar (ejemplo: obtener el último ID insertado).
        /// </summary>
        public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null,
            CommandType commandType = CommandType.Text)
        {
            var (conn, tx, owns) = GetConnAndTx();

            try
            {
                await OpenIfNeededAsync(conn);
                var result = await conn.ExecuteScalarAsync(new CommandDefinition(sql, param, tx, commandType: commandType));
                if (result == null || result == DBNull.Value) return default!;
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al ejecutar ExecuteScalarAsync: {ex.Message}");
            }
            finally
            {
                await CloseIfOwnedAsync(conn, owns);
            }
        }

        #endregion
    }
}
