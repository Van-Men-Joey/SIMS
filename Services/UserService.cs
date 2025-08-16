using SIMS.Interfaces;
using SIMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ICourseRepository _courseRepo;

        public UserService(IUserRepository repo, IEnrollmentRepository enrollmentRepo, ICourseRepository courseRepo)
        {
            _repo = repo;
            _enrollmentRepo = enrollmentRepo;
            _courseRepo = courseRepo;
        }

        public Task<IEnumerable<User>> GetAllAsync() => _repo.GetAllAsync();

        public Task<User?> GetByIdAsync(string id) => _repo.GetByIdAsync(id);

        public Task<User?> GetByEmailAsync(string email) => _repo.GetByEmailAsync(email);

        // Thêm user với ID đã tạo sẵn (ít dùng)
        public Task AddAsync(User user)
        {
            return _repo.AddAsync(user);
        }

        // Thêm user với việc tự tạo ID đẹp có prefix, trên CSV
        public async Task AddNewUserAsync(User user)
        {
            string prefix = user.Role switch
            {
                UserRole.Student => "ST",
                UserRole.Faculty => "FC",
                UserRole.Admin => "AD",
                _ => "US"
            };

            var users = await _repo.GetAllAsync();
            var filteredUsers = users.Where(u => u.Id.StartsWith(prefix)).ToList();

            if (!filteredUsers.Any())
            {
                user.Id = prefix + "001";
            }
            else
            {
                var lastUser = filteredUsers.OrderByDescending(u => u.Id).First();
                string lastNumberStr = lastUser.Id.Substring(prefix.Length);
                int lastNumber = 0;
                int.TryParse(lastNumberStr, out lastNumber);
                int newNumber = lastNumber + 1;
                user.Id = prefix + newNumber.ToString("D3");
            }

            await _repo.AddAsync(user);
        }

        public Task UpdateAsync(User user) => _repo.UpdateAsync(user);

        // Xóa user và các dữ liệu liên quan (Enrollments, Courses) cũng dùng CSV
        public async Task DeleteAsync(string id)
        {
            var user = await _repo.GetByIdAsync(id);
            if (user == null) return;

            if (user is Student student)
            {
                var enrollments = (await _enrollmentRepo.GetAllAsync())
                    .Where(e => e.StudentId == student.Id)
                    .ToList();

                foreach (var e in enrollments)
                {
                    await _enrollmentRepo.DeleteAsync(e.Id);
                }
            }

            if (user is Faculty faculty)
            {
                var courses = (await _courseRepo.GetAllAsync())
                    .Where(c => c.FacultyId == faculty.Id)
                    .ToList();

                foreach (var c in courses)
                {
                    await _courseRepo.DeleteAsync(c.Id);
                }
            }

            await _repo.DeleteAsync(id);
        }

        public Task<IEnumerable<Faculty>> GetAllFacultiesAsync()
        {
            return _repo.GetAllFacultiesAsync();
        }
    }
}
