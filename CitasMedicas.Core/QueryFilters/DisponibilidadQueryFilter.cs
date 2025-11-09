using System;

namespace CitasMedicas.Core.QueryFilters
{
    public class DisponibilidadQueryFilter
    {
        public int? MedicoId { get; set; }
        public DateTime? Fecha { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public decimal? CostoCita { get; set; }
    }
}

