using SIMS.Interfaces;
using SIMS.Models;
using SIMS.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace SIMS.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository _authRepository;

        public AuthenticationService(IAuthenticationRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null) return null;

            if (BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return user;

            return null;
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            var existing = await _authRepository.GetUserByEmailAsync(user.Email);
            if (existing != null) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            await _authRepository.AddUserAsync(user);
            return true;
        }

    }
}
