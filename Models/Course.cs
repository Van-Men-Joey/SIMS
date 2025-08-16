using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Course
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, StringLength(200)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string? FacultyId { get; set; }
        [ForeignKey(nameof(FacultyId))]
        public Faculty? Faculty { get; set; }

        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Assignment> Assignments { get; set; } = new();
    }
}
//Falculty quản lý môn học của mình
//Quản lý môn học của mình
//Xem danh sách các khóa học mà giáo viên phụ trách (CourseManagementService - chỉ đọc các khóa của mình).

//Cập nhật mô tả hoặc tài liệu môn học.

//Admin quản lý môn học
//Thêm, sửa, xóa khóa học (CourseManagementService).

//Phân công giáo viên cho môn học.

//Xóa hoặc lưu trữ môn học cũ.
