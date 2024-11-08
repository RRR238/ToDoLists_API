using BCrypt.Net;

namespace ToDo_lists.Services
{
    public class PasswordHash
    {
        public string HashPassword(string plainPassword)
        {
            // Generates a salt and hashes the password
            return BCrypt.Net.BCrypt.HashPassword(plainPassword);
        }

        // Verify a password
        public bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);
        }
    }
}
