using System;
using System.Data;
using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using CitasMedicas.Core.Enums;
using CitasMedicas.Core.Interfaces;

namespace CitasMedicas.Infrastructure.Repositories
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _config;
        private readonly string _sqlConn;
        private readonly string _mySqlConn;

        public DatabaseProvider Provider { get; }

        public DbConnectionFactory(IConfiguration config)
        {
            _config = config;
            _sqlConn = _config.GetConnectionString("ConnectionSqlServer") ?? string.Empty;
            _mySqlConn = _config.GetConnectionString("ConnectionMySql") ?? string.Empty;

            var providerStr = _config.GetSection("DatabaseProvider").Value ?? "SqlServer";

            Provider = providerStr.Equals("MySql", StringComparison.OrdinalIgnoreCase)
                ? DatabaseProvider.MySql
                : DatabaseProvider.SqlServer;
        }

        public IDbConnection CreateConnection()
        {
            return Provider switch
            {
                DatabaseProvider.MySql => new MySqlConnection(_mySqlConn),
                _ => new SqlConnection(_sqlConn)
            };
        }
    }
}
