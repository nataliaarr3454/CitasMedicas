using System;

namespace CitasMedicas.Core.QueryFilters
{
    public class CitaQueryFilter : PaginationQueryFilter
    {
        public int? PacienteId { get; set; }
        public int? MedicoId { get; set; }
        public string? Estado { get; set; }
        public DateTime? FechaCita { get; set; }
    }
}
