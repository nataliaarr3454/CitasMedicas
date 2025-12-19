using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para realizar la reserva de una cita médica.
    /// </summary>
    public class ReservaCitaDto
    {
        /// <summary>
        /// Correo electrónico del paciente que reserva la cita.
        /// </summary>
        /// <example>paciente.ejemplo@email.com</example>
        public string CorreoPaciente { get; set; } = null!;

        /// <summary>
        /// Identificador de la disponibilidad seleccionada para la cita.
        /// </summary>
        /// <example>12</example>
        public int DisponibilidadId { get; set; }

        /// <summary>
        /// Motivo de la consulta médica.
        /// </summary>
        /// <example>Consulta de rutina anual</example>
        public string Motivo { get; set; } = null!;
    }
}
