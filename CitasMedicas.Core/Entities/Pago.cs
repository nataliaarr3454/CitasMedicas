using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    public class Pago : BaseEntity
    {
        public int CitaId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; } = DateTime.Now;
        public string EstadoPago { get; set; } = "Pendiente";// Pendiente, Pagado, Multado
        public Cita Cita { get; set; } = null!;

    }
}
