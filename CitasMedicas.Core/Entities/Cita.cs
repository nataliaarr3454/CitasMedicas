using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    public class Cita : BaseEntity
    {
        public int PacienteId { get; set; }
        public int MedicoId { get; set; }
        public int DisponibilidadId { get; set; }
        public DateTime FechaCita { get; set; }
        public TimeSpan HoraCita { get; set; }
        public string Motivo { get; set; } = null!;
        public string Estado { get; set; } = "Reservada";
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public Paciente Paciente { get; set; } = null!;
        public Medico Medico { get; set; } = null!;
        public Disponibilidad Disponibilidad { get; set; } = null!;
        public Pago? Pago { get; set; } 

    }
}

