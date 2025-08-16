using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(string id);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetByCourseIdAsync(string courseId);
        Task AddAsync(Enrollment enrollment);
        Task DeleteAsync(string id);
        Task UpdateAsync(Enrollment enrollment); // ✅ Thêm dòng này
    }
}
