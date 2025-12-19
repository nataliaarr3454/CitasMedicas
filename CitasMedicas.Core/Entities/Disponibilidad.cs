using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa la disponibilidad horaria de un médico para atender citas.
    /// </summary>
    public class Disponibilidad : BaseEntity
    {
        /// <summary>
        /// Identificador del médico al que pertenece esta disponibilidad.
        /// </summary>
        /// <example>3</example>
        public int MedicoId { get; set; }

        /// <summary>
        /// Fecha en la que el médico está disponible.
        /// </summary>
        /// <example>2025-06-15</example>
        public DateTime Fecha { get; set; }

        /// <summary>
        /// Hora de inicio de la disponibilidad.
        /// </summary>
        /// <example>09:00:00</example>
        public TimeSpan HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin de la disponibilidad.
        /// </summary>
        /// <example>10:00:00</example>
        public TimeSpan HoraFin { get; set; }

        /// <summary>
        /// Costo de la cita durante esta disponibilidad.
        /// </summary>
        /// <example>50.00</example>
        public decimal CostoCita { get; set; }

        /// <summary>
        /// Estado de la disponibilidad (Disponible / Ocupado).
        /// </summary>
        /// <example>Disponible</example>
        public string Estado { get; set; } = "Disponible"; //Disponible / Ocupado

        /// <summary>
        /// Referencia al médico asociado.
        /// </summary>
        public Medico Medico { get; set; } = null!;

        /// <summary>
        /// Colección de citas asociadas a esta disponibilidad.
        /// </summary>
        public ICollection<Cita>? Citas { get; set; }
    }
}
