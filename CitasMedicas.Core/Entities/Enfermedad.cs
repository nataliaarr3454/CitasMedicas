using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CitasMedicas.Core.Entities
{
    public class Enfermedad : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public ICollection<PacienteEnfermedad>? PacientesEnfermedades { get; set; }

    }
}
