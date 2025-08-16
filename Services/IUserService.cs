using SIMS.Models;

namespace SIMS.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(string id);
        Task<User?> GetByEmailAsync(string email);
        Task AddAsync(User user);
        // Thêm user với ID tự động tạo (nên dùng cái này khi tạo mới user)
        Task AddNewUserAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(string id);
        Task<IEnumerable<Faculty>> GetAllFacultiesAsync();
    }
}
