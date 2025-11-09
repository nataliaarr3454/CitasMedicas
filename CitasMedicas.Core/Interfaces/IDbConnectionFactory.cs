using System.Data;
using CitasMedicas.Core.Enums;

namespace CitasMedicas.Core.Interfaces
{
    public interface IDbConnectionFactory
    {
        DatabaseProvider Provider { get; }
        IDbConnection CreateConnection();
    }
}
