using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password);
        bool Check(string hash, string password);
    }
}