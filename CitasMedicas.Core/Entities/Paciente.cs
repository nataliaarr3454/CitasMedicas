using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa un paciente del sistema de citas médicas.
    /// </summary>
    public class Paciente : BaseEntity
    {
        /// <summary>
        /// Nombre del paciente.
        /// </summary>
        /// <example>María</example>
        public string Nombre { get; set; } = null!;

        /// <summary>
        /// Apellido del paciente.
        /// </summary>
        /// <example>González</example>
        public string Apellido { get; set; } = null!;

        /// <summary>
        /// Edad del paciente en años.
        /// </summary>
        /// <example>35</example>
        public int Edad { get; set; }

        /// <summary>
        /// Correo electrónico de contacto del paciente.
        /// </summary>
        /// <example>maria.gonzalez@email.com</example>
        public string Correo { get; set; } = null!;

        /// <summary>
        /// Número de teléfono de contacto del paciente.
        /// </summary>
        /// <example>+5491123456789</example>
        public string Telefono { get; set; } = null!;

        /// <summary>
        /// Dirección de residencia del paciente.
        /// </summary>
        /// <example>Calle Principal 123, Ciudad, País</example>
        public string Direccion { get; set; } = null!;

        /// <summary>
        /// Saldo disponible del paciente para pagar citas.
        /// </summary>
        /// <example>150.50</example>
        public decimal Saldo { get; set; } = 0;

        /// <summary>
        /// Colección de citas programadas por el paciente.
        /// </summary>
        public ICollection<Cita>? Citas { get; set; }
    }
}