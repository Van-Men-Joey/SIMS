using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface IAssignmentRepository
    {
        Task<IEnumerable<Assignment>> GetAllAsync();
        Task<Assignment?> GetByIdAsync(string id);
        Task<IEnumerable<Assignment>> GetByCourseIdAsync(string courseId);
        Task AddAsync(Assignment assignment);
        Task UpdateAsync(Assignment assignment);
        Task DeleteAsync(string id);
    }
}
