using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitasMedicas.Core.Entities
{
    /// <summary>
    /// Entidad utilizado para autenticacion de usuarios.
    /// </summary>
    /// <remarks>
    /// Se utiliza en el proceso de login para obtener un token JWT.
    /// </remarks>
    public class UserLogin
    {
        /// <summary>
        /// Usuario o login.
        /// </summary>
        /// <example>admin</example>
        public string Login { get; set; }
        /// <summary>
        /// Contraseña del usuario.
        /// </summary>
        /// <example>admin123</example>
        public string Password { get; set; }
    }
}