using CitasMedicas.Core.Enums;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Representa un usuario del sistema
    /// </summary>
    public class Security : BaseEntity
    {
        /// <summary>
        /// Login del usuario.
        /// </summary>
        /// <example>medic</example>
        public string Login { get; set; }
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        /// <example>medi123</example>
        public string Password { get; set; }
        /// <summary>
        /// Nombre completo del usuario.
        /// </summary>
        /// <example>Medico</example>
        public string Name { get; set; }
        /// <summary>
        /// Rol asignado al usuario.
        /// </summary>
        /// <example>Medico</example>
        public RoleType Role { get; set; }
    }
}