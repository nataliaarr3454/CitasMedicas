using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    public class Medico : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Especialidad { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Telefono { get; set; } = null!;
        public ICollection<Disponibilidad>? Disponibilidades { get; set; }
        public ICollection<Cita>? Citas { get; set; }

    }
}
