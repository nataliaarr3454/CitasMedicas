using CitasMedicas.Core.Enums;

namespace CitasMedicas.Core.Entities
{
    public class Security : BaseEntity
    {
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public RoleType Role { get; set; }
    }
}