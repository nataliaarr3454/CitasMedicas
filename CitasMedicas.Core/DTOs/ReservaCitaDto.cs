namespace CitasMedicas.Core.DTOs
{
    public class ReservaCitaDto
    {
        public string CorreoPaciente { get; set; } = null!;
        public int DisponibilidadId { get; set; }
        public string Motivo { get; set; } = null!;
    }

}
