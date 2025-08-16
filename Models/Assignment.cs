using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Assignment
    {
        [Key]
        public string? Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public List<Submission> Submissions { get; set; } = new();
        // Thêm ngày tạo bài tập
        public DateTime CreatedDate { get; set; }

        // Thêm hạn nộp bài
        public DateTime DueDate { get; set; }
    }
}
//Quản lý bài tập
//Tạo bài tập mới (Assignment → lưu qua SubmissionService hoặc CourseManagementService).

//Cập nhật, chỉnh sửa, xóa bài tập của mình.

//Đặt thời hạn nộp.