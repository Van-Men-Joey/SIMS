using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repo;

        public ReportService(IReportRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Report>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Report?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
        public Task<IEnumerable<Report>> GetByStudentIdAsync(string studentId) => _repo.GetByStudentIdAsync(studentId);
        public Task<IEnumerable<Report>> GetByCourseIdAsync(string courseId) => _repo.GetByCourseIdAsync(courseId);
        public Task AddAsync(Report report) => _repo.AddAsync(report);
        public Task UpdateAsync(Report report) => _repo.UpdateAsync(report);
        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);
    }
}
