using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa un médico del sistema de citas médicas.
    /// </summary>
    public class Medico : BaseEntity
    {
        /// <summary>
        /// Nombre completo del médico.
        /// </summary>
        /// <example>Dr. Juan Pérez</example>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Especialidad médica del profesional.
        /// </summary>
        /// <example>Cardiología</example>
        public string Especialidad { get; set; } = null!;

        /// <summary>
        /// Correo electrónico de contacto del médico.
        /// </summary>
        /// <example>juan.perez@clinica.com</example>
        public string Correo { get; set; } = null!;

        /// <summary>
        /// Número de teléfono de contacto del médico.
        /// </summary>
        /// <example>+1234567890</example>
        public string Telefono { get; set; } = null!;

        /// <summary>
        /// Colección de disponibilidades horarias del médico.
        /// </summary>
        public ICollection<Disponibilidad>? Disponibilidades { get; set; }

        /// <summary>
        /// Colección de citas programadas con el médico.
        /// </summary>
        public ICollection<Cita>? Citas { get; set; }
    }
}