using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para cancelar una cita médica.
    /// </summary>
    public class CancelarCitaDto
    {
        /// <summary>
        /// Motivo por el cual se cancela la cita.
        /// </summary>
        /// <example>El paciente no puede asistir por motivos personales.</example>
        public string? MotivoCancelacion { get; set; }
    }
}
