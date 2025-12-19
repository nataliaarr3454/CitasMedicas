using CitasMedicas.Core.CustomEntities;
using CitasMedicas.Core.Interfaces;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace CitasMedicas.Core.Services
{
    public class PasswordService : IPasswordService
    {
        private readonly PasswordOptions _options;

        public PasswordService(IOptions<PasswordOptions> options)
        {
            _options = options.Value;
        }
        public string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(_options.SaltSize);

            var key = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                _options.Iterations,
                HashAlgorithmName.SHA256,
                _options.KeySize
            );

            return $"{_options.Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(key)}";
        }
        public bool Check(string hash, string password)
        {
            var parts = hash.Split('.');
            if (parts.Length != 3)
                throw new FormatException("El formato del Hash no es correcta");


            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var keyToCheck = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                _options.KeySize
            );

            return CryptographicOperations.FixedTimeEquals(keyToCheck, key);
        }
    }
}