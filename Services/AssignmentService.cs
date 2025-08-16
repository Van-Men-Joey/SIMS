using SIMS.Interfaces;
using SIMS.Models;

namespace SIMS.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _repo;

        public AssignmentService(IAssignmentRepository repo)
        {
            _repo = repo;
        }

        public Task<IEnumerable<Assignment>> GetAllAsync() => _repo.GetAllAsync();
        public Task<Assignment?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);
        public Task<IEnumerable<Assignment>> GetByCourseIdAsync(string courseId) => _repo.GetByCourseIdAsync(courseId);
        public Task AddAsync(Assignment assignment) => _repo.AddAsync(assignment);
        public Task UpdateAsync(Assignment assignment) => _repo.UpdateAsync(assignment);
        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);
    }
}
