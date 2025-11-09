using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    public class PacienteEnfermedad : BaseEntity
    {
        public int PacienteId { get; set; }
        public int EnfermedadId { get; set; }
        public string Tratamiento { get; set; } = null!;
        public string Medicacion { get; set; } = null!;

        public Paciente Paciente { get; set; } = null!;
        public Enfermedad Enfermedad { get; set; } = null!;
       
    }
}

