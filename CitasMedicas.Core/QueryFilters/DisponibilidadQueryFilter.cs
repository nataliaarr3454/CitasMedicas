using System;

namespace CitasMedicas.Core.QueryFilters
{
    /// <summary>
    /// Filtros para consulta de disponibilidades médicas
    /// </summary>
    public class DisponibilidadQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// ID del médico
        /// </summary>
        /// <example>1</example>
        public int? MedicoId { get; set; }

        /// <summary>
        /// Fecha de la disponibilidad
        /// </summary>
        /// <example>2025-01-15</example>
        public DateTime? Fecha { get; set; }

        /// <summary>
        /// Hora de inicio de la disponibilidad
        /// </summary>
        /// <example>09:00:00</example>
        public TimeSpan? HoraInicio { get; set; }

        /// <summary>
        /// Hora de fin de la disponibilidad
        /// </summary>
        /// <example>10:00:00</example>
        public TimeSpan? HoraFin { get; set; }

        /// <summary>
        /// Costo de la cita
        /// </summary>
        /// <example>150.00</example>
        public decimal? CostoCita { get; set; }
    }
}