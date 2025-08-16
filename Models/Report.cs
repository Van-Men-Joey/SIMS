using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.Models
{
    public class Report
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        public string StudentId { get; set; }
        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; }

        [Required]
        public string CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public Course? Course { get; set; }

        public string Content { get; set; }
    }
}
//Falculty quản lý báo cáo
//Báo cáo & theo dõi
//Xem thống kê kết quả học tập của sinh viên trong lớp mình (ReportService).

//Xuất báo cáo điểm cho lớp mình.

//Admin quản lý báo cáo
//Báo cáo hệ thống
//Xem báo cáo tổng hợp toàn trường (ReportService).

//Xuất dữ liệu toàn hệ thống (tất cả môn học, tất cả khoa).