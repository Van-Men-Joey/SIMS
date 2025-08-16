using System.ComponentModel.DataAnnotations;

namespace SIMS.Models
{
    public abstract class User
    {
       
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [Required, StringLength(255)]
        public string Name { get; set; }

        public string? Description { get; set; }
        
    }

    public enum UserRole
    {
        Admin,
        Student,
        Faculty
    }
   //Admin quản lý người dùng
//Thêm, sửa, xóa tài khoản(UserManagementService).
//Phân quyền: gán vai trò Teacher, Student, hoặc Admin.
//Reset mật khẩu.
}
