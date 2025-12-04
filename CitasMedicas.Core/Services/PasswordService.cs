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
            byte[] salt = RandomNumberGenerator.GetBytes(_options.SaltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                _options.Iterations,
                HashAlgorithmName.SHA256,
                _options.KeySize
            );

            var hashBase64 = Convert.ToBase64String(hash);
            var saltBase64 = Convert.ToBase64String(salt);

            return $"{_options.Iterations}.{saltBase64}.{hashBase64}";
        }

        public bool Check(string hash, string password)
        {
            try
            {
                var parts = hash.Split('.');
                if (parts.Length != 3)
                {
                    throw new FormatException("El formato del hash no es correcto.");
                }

                var iterations = Convert.ToInt32(parts[0]);
                var salt = Convert.FromBase64String(parts[1]);
                var storedHash = Convert.FromBase64String(parts[2]);

                byte[] calculatedHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    _options.KeySize
                );

                return CryptographicOperations.FixedTimeEquals(calculatedHash, storedHash);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}