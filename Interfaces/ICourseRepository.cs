using SIMS.Models;

namespace SIMS.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(string id);
        Task<IEnumerable<Course>> GetByFacultyIdAsync(string facultyId);
        Task<IEnumerable<Course>> GetByStudentIdAsync(string studentId);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(string id);
    }
}
