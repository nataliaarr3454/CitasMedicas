using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa el pago asociado a una cita médica.
    /// </summary>
    public class Pago : BaseEntity
    {
        /// <summary>
        /// Identificador de la cita asociada al pago.
        /// </summary>
        /// <example>15</example>
        public int CitaId { get; set; }

        /// <summary>
        /// Monto del pago.
        /// </summary>
        /// <example>75.00</example>
        public decimal Monto { get; set; }

        /// <summary>
        /// Fecha y hora en que se realizó el pago.
        /// </summary>
        /// <example>2025-06-10T14:30:00</example>
        public DateTime FechaPago { get; set; } = DateTime.Now;

        /// <summary>
        /// Estado actual del pago (Pendiente, Pagado, Multado).
        /// </summary>
        /// <example>Pagado</example>
        public string EstadoPago { get; set; } = "Pendiente";

        /// <summary>
        /// Referencia a la cita asociada al pago.
        /// </summary>
        public Cita Cita { get; set; } = null!;
    }
}