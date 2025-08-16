using SIMS.Models;
using System.ComponentModel.DataAnnotations;

namespace SIMS.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; } 

        [Required(ErrorMessage = "Tên không được để trống.")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Xác nhận mật khẩu không khớp.")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public UserRole Role { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
