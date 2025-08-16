using SIMS.Models;

namespace SIMS.Services
{
    public interface IAssignmentService
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(string id);
        Task<IEnumerable<Assignment>> GetByCourseIdAsync(string courseId);
        Task AddAsync(Assignment assignment);
        Task UpdateAsync(Assignment assignment);
        Task DeleteAsync(string id);
    }
}
