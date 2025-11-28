using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using SignalR.Models;

namespace SignalR.Services
{
    public class InMemoryUserStore
    {
        private readonly ConcurrentDictionary<string, UserAccount> _users = new(StringComparer.OrdinalIgnoreCase);

        public bool TryAddUser(string username, string password, out string? error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            var normalized = username.Trim();
            if (_users.ContainsKey(normalized))
            {
                error = "A user with that name already exists.";
                return false;
            }

            var account = new UserAccount
            {
                Id = Guid.NewGuid().ToString(),
                UserName = normalized,
                PasswordHash = HashPassword(password),
                CreatedAt = DateTime.UtcNow
            };

            var added = _users.TryAdd(normalized, account);
            if (!added)
            {
                error = "Unable to create user. Please try again.";
            }

            return added;
        }

        public bool TryValidateUser(string username, string password, out UserAccount? account)
        {
            account = null;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            if (!_users.TryGetValue(username.Trim(), out var existing))
            {
                return false;
            }

            var hash = HashPassword(password);
            if (!string.Equals(existing.PasswordHash, hash, StringComparison.Ordinal))
            {
                return false;
            }

            account = existing;
            return true;
        }

        public IReadOnlyCollection<string> GetAllUserNames()
        {
            return _users.Keys.OrderBy(name => name).ToList();
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}
