namespace CitasMedicas.Core.DTOs
{
    public class DisponibilidadDto
    {
        public int MedicoId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public decimal CostoCita { get; set; }
    }
}