using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.DTOs
{
    /// <summary>
    /// DTO para crear o actualizar un paciente.
    /// </summary>
    public class PacienteDto
    {
        /// <summary>
        /// Nombre del paciente.
        /// </summary>
        /// <example>Laura</example>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Apellido del paciente.
        /// </summary>
        /// <example>Martínez</example>
        public string Apellido { get; set; } = null!;

        /// <summary>
        /// Edad del paciente en años.
        /// </summary>
        /// <example>28</example>
        public int Edad { get; set; }

        /// <summary>
        /// Correo electrónico de contacto.
        /// </summary>
        /// <example>laura.martinez@email.com</example>
        public string Correo { get; set; } = null!;

        /// <summary>
        /// Número de teléfono de contacto.
        /// </summary>
        /// <example>79842366</example>
        public string Telefono { get; set; } = null!;

        /// <summary>
        /// Dirección de residencia.
        /// </summary>
        /// <example>Avenida Siempre Viva 742, Springfield</example>
        public string Direccion { get; set; } = null!;
    }
}