using SIMS.Models;

namespace SIMS.Services
{
    public interface IEnrollmentService
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(string id);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId);
        Task UpdateAsync(Enrollment enrollment); 
        Task AddAsync(Enrollment enrollment);
        Task DeleteAsync(string id);
    }
}
