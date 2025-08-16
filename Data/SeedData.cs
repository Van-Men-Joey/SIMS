using BCrypt.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIMS.Interfaces;
using SIMS.Models;
using System;
using System.Linq;

namespace SIMS.Data
{
    public static class SeedDataCsv
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var assignmentRepo = serviceProvider.GetRequiredService<IAssignmentRepository>();
            var submissionRepo = serviceProvider.GetRequiredService<ISubmissionRepository>();
            var courseRepo = serviceProvider.GetRequiredService<ICourseRepository>();
            var enrollmentRepo = serviceProvider.GetRequiredService<IEnrollmentRepository>();
            var reportRepo = serviceProvider.GetRequiredService<IReportRepository>();
            var userRepo = serviceProvider.GetRequiredService<IUserRepository>();

            var users = await userRepo.GetAllAsync();
            if (!users.Any())
            {
                string adminPass = BCrypt.Net.BCrypt.HashPassword("admin123");
                string facultyPass = BCrypt.Net.BCrypt.HashPassword("faculty123");
                string studentPass = BCrypt.Net.BCrypt.HashPassword("student123");

                var admin = new Admin
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "System Admin",
                    Email = "admin@university.edu",
                    PasswordHash = adminPass,
                    Role = UserRole.Admin
                };
                var faculty = new Faculty
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Faculty of Computer Science",
                    Email = "faculty@university.edu",
                    PasswordHash = facultyPass,
                    Role = UserRole.Faculty
                };
                var student = new Student
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "John Doe",
                    Email = "john@student.edu",
                    PasswordHash = studentPass,
                    Role = UserRole.Student
                };

                await userRepo.AddAsync(admin);
                await userRepo.AddAsync(faculty);
                await userRepo.AddAsync(student);
            }

            var faculties = await userRepo.GetAllAsync();
            var facultyId = faculties.FirstOrDefault(u => u.Role == UserRole.Faculty)?.Id;
            var students = faculties.Where(u => u.Role == UserRole.Student).ToList();
            var studentId = students.FirstOrDefault()?.Id;

            var courses = await courseRepo.GetAllAsync();
            if (!courses.Any() && facultyId != null)
            {
                var course = new Course
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Introduction to Programming",
                    Description = "Basic programming concepts",
                    FacultyId = facultyId
                };
                await courseRepo.AddAsync(course);
            }

            var courseId = (await courseRepo.GetAllAsync()).FirstOrDefault()?.Id;

            var enrollments = await enrollmentRepo.GetAllAsync();
            if (!enrollments.Any() && studentId != null && courseId != null)
            {
                var enrollment = new Enrollment
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    CourseId = courseId,
                    EnrolledDate = DateTime.UtcNow
                };
                await enrollmentRepo.AddAsync(enrollment);
            }

            var assignments = await assignmentRepo.GetAllAsync();
            if (!assignments.Any() && courseId != null)
            {
                var assignment = new Assignment
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Assignment 1",
                    Description = "First programming task",
                    CourseId = courseId
                };
                await assignmentRepo.AddAsync(assignment);
            }

            var assignmentId = (await assignmentRepo.GetAllAsync()).FirstOrDefault()?.Id;

            var submissions = await submissionRepo.GetAllAsync();
            if (!submissions.Any() && studentId != null && assignmentId != null)
            {
                var submission = new Submission
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    AssignmentId = assignmentId,
                    FilePath = "/submissions/assignment1.docx",
                    Score = 90
                };
                await submissionRepo.AddAsync(submission);
            }

            var reports = await reportRepo.GetAllAsync();
            if (!reports.Any() && studentId != null && courseId != null)
            {
                var report = new Report
                {
                    Id = Guid.NewGuid().ToString(),
                    StudentId = studentId,
                    CourseId = courseId,
                    Content = "Excellent performance"
                };
                await reportRepo.AddAsync(report);
            }
        }
    }

}

