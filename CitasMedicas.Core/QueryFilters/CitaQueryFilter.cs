using System;

namespace CitasMedicas.Core.QueryFilters
{
    /// <summary>
    /// Filtros para consulta de citas médicas
    /// </summary>
    public class CitaQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// ID del paciente
        /// </summary>
        /// <example>1</example>
        public int? PacienteId { get; set; }

        /// <summary>
        /// ID del médico
        /// </summary>
        /// <example>2</example>
        public int? MedicoId { get; set; }

        /// <summary>
        /// Estado de la cita (Ej: Reservada, Completada, Cancelada)
        /// </summary>
        /// <example>Reservada</example>
        public string? Estado { get; set; }

        /// <summary>
        /// Fecha de la cita
        /// </summary>
        /// <example>2025-01-15</example>
        public DateTime? FechaCita { get; set; }
    }
}
