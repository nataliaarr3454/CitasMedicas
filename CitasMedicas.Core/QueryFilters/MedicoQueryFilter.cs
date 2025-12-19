namespace CitasMedicas.Core.QueryFilters
{
    /// <summary>
    /// Filtros para consulta de médicos
    /// </summary>
    public class MedicoQueryFilter : PaginationQueryFilter
    {
        /// <summary>
        /// ID del médico
        /// </summary>
        /// <example>1</example>
        public int? MedicoId { get; set; }

        /// <summary>
        /// Nombre del médico
        /// </summary>
        /// <example>Juan</example>
        public string? Nombre { get; set; }

        /// <summary>
        /// Especialidad del médico
        /// </summary>
        /// <example>Cardiología</example>
        public string? Especialidad { get; set; }

        /// <summary>
        /// Teléfono del médico
        /// </summary>
        /// <example>77788899</example>
        public string? Telefono { get; set; }
    }
}