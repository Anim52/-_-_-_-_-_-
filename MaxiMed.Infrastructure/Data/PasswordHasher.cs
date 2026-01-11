using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    public static class PasswordHasher
    {
        private const int SaltSize = 16;
        private const int KeySize = 32;
        private const int DefaultIterations = 100_000;

        public static string Hash(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                DefaultIterations,
                HashAlgorithmName.SHA256);

            var hash = pbkdf2.GetBytes(KeySize);

            return $"{DefaultIterations}." +
                   $"{Convert.ToBase64String(salt)}." +
                   $"{Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            var parts = stored.Split('.');
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            using var pbkdf2 = new Rfc2898DeriveBytes(
                password,
                salt,
                iterations,
                HashAlgorithmName.SHA256);

            var computed = pbkdf2.GetBytes(hash.Length);
            return CryptographicOperations.FixedTimeEquals(computed, hash);
        }
    }
}
