using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    public class Paciente : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public int Edad { get; set; }
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public string Direccion { get; set; } = null!;
        public decimal Saldo { get; set; } = 0;
        public ICollection<Cita>? Citas { get; set; }
       

    }
}
