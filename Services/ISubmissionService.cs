using SIMS.Models;

namespace SIMS.Services
{
    public interface ISubmissionService
    {
        Task<IEnumerable<Submission>> GetByCourseIdAsync(string courseId);
        Task<IEnumerable<Submission>> GetAllAsync();
        Task<Submission?> GetByIdAsync(string id);
        Task<IEnumerable<Submission>> GetByAssignmentIdAsync(string assignmentId);
        Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId);
        Task AddAsync(Submission submission);
        Task UpdateAsync(Submission submission);
        Task DeleteAsync(string id);
    }
}
