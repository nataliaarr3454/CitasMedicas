using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para mostrar información básica de un médico.
    /// </summary>
    public class MedicoDto
    {
        /// <summary>
        /// Identificador único del médico.
        /// </summary>
        /// <example>7</example>
        public int Id { get; set; }

        /// <summary>
        /// Nombre completo del médico.
        /// </summary>
        /// <example>Dr. Carlos Rodríguez</example>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Especialidad médica del profesional.
        /// </summary>
        /// <example>Dermatología</example>
        public string Especialidad { get; set; } = null!;

        /// <summary>
        /// Correo electrónico de contacto.
        /// </summary>
        /// <example>carlos.rodriguez@clinica.com</example>
        public string Correo { get; set; } = null!;

        /// <summary>
        /// Número de teléfono de contacto.
        /// </summary>
        /// <example>+5491154876321</example>
        public string Telefono { get; set; } = null!;
    }
}