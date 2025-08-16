using SIMS.Models;
using System.Threading.Tasks;


namespace SIMS.Services
{
    public interface IAuthenticationService
    {
        Task<User?> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(User user, string password);
    }
}
