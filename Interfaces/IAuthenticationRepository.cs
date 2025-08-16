using SIMS.Models;
using System.Threading.Tasks;

namespace SIMS.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
    }
}
