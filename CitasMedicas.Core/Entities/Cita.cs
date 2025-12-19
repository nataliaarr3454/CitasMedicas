using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa las citas medicas on line
    /// </summary>
    /// <remarks>
    /// Esta entidad almacena la informacion principal de una cita
    /// </remarks>
    public class Cita : BaseEntity
    {
        /// <summary>
        /// Identificador del paciente que tiene la cita programada.
        /// </summary>
        /// <example>5</example>
        public int PacienteId { get; set; }
        /// <summary>
        /// Identificador del médico que tiene la cita programada.
        /// </summary>
        /// <example>2</example>
        public int MedicoId { get; set; }
        /// <summary>
        /// Identificador de la disponibilidad utilizada para la cita.
        /// </summary>
        /// <example>10</example>
        public int DisponibilidadId { get; set; }
        /// <summary>
        /// Fecha programada de la cita.
        /// </summary>
        /// <example>2025-06-11</example>
        public DateTime FechaCita { get; set; }
        /// <summary>
        /// Hora programada de la cita.
        /// </summary>
        /// <example>14:30:00</example>
        public TimeSpan HoraCita { get; set; }
        /// <summary>
        /// Motivo de la consulta.
        /// </summary>
        /// <example>Control de rutina</example>
        public string Motivo { get; set; } = null!;
        /// <summary>
        /// Estado de la Cita.
        /// </summary>
        /// <example>Reservada</example>
        public string Estado { get; set; } = "Reservada";
        /// <summary>
        /// Fecha y hora en que se registró la cita.
        /// </summary>
        /// <example>2025-06-10T09:15:00</example>
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        /// <summary>
        /// Referencia al paciente asociado.
        /// </summary>
        public Paciente Paciente { get; set; } = null!;
        /// <summary>
        /// Referencia al médico asociado.
        /// </summary>
        public Medico Medico { get; set; } = null!;
        /// <summary>
        /// Referencia a la disponibilidad asociada.
        /// </summary>
        public Disponibilidad Disponibilidad { get; set; } = null!;
        /// <summary>
        /// Referencia al pago asociado (opcional).
        /// </summary>
        public Pago? Pago { get; set; } 

    }
}

