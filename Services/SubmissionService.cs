using SIMS.Interfaces;
using SIMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _repo;
        private readonly IUserRepository _userRepo;
        private readonly IAssignmentRepository _assignmentRepo;

        public SubmissionService(
            ISubmissionRepository repo,
            IUserRepository userRepo,
            IAssignmentRepository assignmentRepo)
        {
            _repo = repo;
            _userRepo = userRepo;
            _assignmentRepo = assignmentRepo;
        }

        public async Task<IEnumerable<Submission>> GetByCourseIdAsync(string courseId)
        {
            var submissions = (await _repo.GetByCourseIdAsync(courseId)).ToList();

            await PopulateRelatedData(submissions);

            return submissions;
        }

        public async Task<IEnumerable<Submission>> GetAllAsync()
        {
            var submissions = (await _repo.GetAllAsync()).ToList();

            await PopulateRelatedData(submissions);

            return submissions;
        }

        public async Task<Submission?> GetByIdAsync(string id)
        {
            var submission = await _repo.GetByIdAsync(id);

            if (submission == null)
                return null;

            // Load Student
            var user = await _userRepo.GetByIdAsync(submission.StudentId);
            if (user is Student student)
            {
                submission.Student = student;
            }
            else
            {
                submission.Student = null;
            }

            // Load Assignment (chưa có trong code trước)
            var assignment = await _assignmentRepo.GetByIdAsync(submission.AssignmentId);
            if (assignment != null)
            {
                submission.Assignment = assignment;
            }
            else
            {
                submission.Assignment = null;
            }

            return submission;
        }


        public async Task<IEnumerable<Submission>> GetByAssignmentIdAsync(string assignmentId)
        {
            var submissions = (await _repo.GetByAssignmentIdAsync(assignmentId)).ToList();

            await PopulateRelatedData(submissions);

            return submissions;
        }

        public async Task<IEnumerable<Submission>> GetByStudentIdAsync(string studentId)
        {
            var submissions = (await _repo.GetByStudentIdAsync(studentId)).ToList();

            await PopulateRelatedData(submissions);

            return submissions;
        }

        public Task AddAsync(Submission submission) => _repo.AddAsync(submission);

        public Task UpdateAsync(Submission submission) => _repo.UpdateAsync(submission);

        public Task DeleteAsync(string id) => _repo.DeleteAsync(id);

        private async Task PopulateRelatedData(List<Submission> submissions)
        {
            var studentIds = submissions.Select(s => s.StudentId).Distinct().ToList();
            var assignmentIds = submissions.Select(s => s.AssignmentId).Distinct().ToList();

            // Lấy toàn bộ Student cùng Assignment một lần để giảm số lần gọi DB hoặc repo
            var students = (await Task.WhenAll(studentIds.Select(id => _userRepo.GetByIdAsync(id))))
                           .Where(u => u != null)
                           .Cast<User>()
                           .ToDictionary(u => u.Id);

            var assignments = (await Task.WhenAll(assignmentIds.Select(id => _assignmentRepo.GetByIdAsync(id))))
                              .Where(a => a != null)
                              .Cast<Assignment>()
                              .ToDictionary(a => a.Id);

            foreach (var submission in submissions)
            {
                if (students.TryGetValue(submission.StudentId, out var user))
                    submission.Student = user as Student;

                if (assignments.TryGetValue(submission.AssignmentId, out var assignment))
                    submission.Assignment = assignment;
            }
        }
    }
}
