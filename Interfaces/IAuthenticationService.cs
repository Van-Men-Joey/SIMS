using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task RegisterAsync(User user, string password);
        Task<bool> UserExistsAsync(string email);
    }
}
