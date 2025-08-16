using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface IReportRepository
    {
        Task<IEnumerable<Report>> GetAllAsync();
        Task<Report?> GetByIdAsync(string id);
        Task<IEnumerable<Report>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Report>> GetByCourseIdAsync(string courseId);
        Task AddAsync(Report report);
        Task UpdateAsync(Report report);
        Task DeleteAsync(string id);
    }
}
