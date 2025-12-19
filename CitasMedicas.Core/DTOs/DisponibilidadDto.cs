using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para crear o actualizar la disponibilidad de un médico.
    /// </summary>
    public class DisponibilidadDto
    {
        /// <summary>
        /// Identificador del médico cuya disponibilidad se está creando.
        /// </summary>
        /// <example>5</example>
        public int MedicoId { get; set; }

        /// <summary>
        /// Fecha en la que el médico está disponible.
        /// </summary>
        /// <example>2025-06-20</example>
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
        /// Costo de la cita durante este horario de disponibilidad.
        /// </summary>
        /// <example>60.00</example>
        public decimal CostoCita { get; set; }
    }
}