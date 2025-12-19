using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para reportes estadísticos de cantidad de citas por médico.
    /// </summary>
    public class MedicoCantidadCitasDto
    {
        /// <summary>
        /// Identificador único del médico.
        /// </summary>
        /// <example>3</example>
        public int MedicoId { get; set; }

        /// <summary>
        /// Nombre completo del médico.
        /// </summary>
        /// <example>Dra. Ana López</example>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Especialidad médica del profesional.
        /// </summary>
        /// <example>Pediatría</example>
        public string Especialidad { get; set; } = null!;

        /// <summary>
        /// Cantidad total de citas atendidas o programadas por el médico.
        /// </summary>
        /// <example>24</example>
        public int CantidadCitas { get; set; }
    }
}
