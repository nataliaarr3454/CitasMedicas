using CitasMedicas.Core.Enums;

namespace CitasMedicas.Core.DTOs
{
    public class SecurityDto
    {
        public string Name { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string Password { get; set; } = null!;
        public RoleType Role { get; set; }
    }
}