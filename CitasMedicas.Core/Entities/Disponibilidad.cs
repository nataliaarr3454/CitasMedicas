using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CitasMedicas.Core.Entities
{
    public class Disponibilidad : BaseEntity
    {
        public int MedicoId { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; }
        public decimal CostoCita { get; set; }
        public string Estado { get; set; } = "Disponible"; // Disponible / Ocupado

        public Medico Medico { get; set; } = null!;
        public ICollection<Cita>? Citas { get; set; }

    }
}
