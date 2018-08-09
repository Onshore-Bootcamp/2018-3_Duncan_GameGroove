using System.ComponentModel.DataAnnotations;

namespace GameGroove.Models
{
    public class ChangePassword
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password must be between 4 and 20 characters", MinimumLength = 4)]
        public string Password { get; set; }

        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password must be between 4 and 20 characters", MinimumLength = 4)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        [StringLength(20, ErrorMessage = "Password must be between 4 and 20 characters", MinimumLength = 4)]
        public string ConfirmNewPassword { get; set; }
    }
}
