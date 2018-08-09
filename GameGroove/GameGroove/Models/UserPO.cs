using System.ComponentModel.DataAnnotations;

namespace GameGroove.Models
{
    public class UserPO
    {
        public int UserID { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [StringLength(35, ErrorMessage = "Names must be 35 characters or less")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        [StringLength(35, ErrorMessage = "Names must be 35 characters or less")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, ErrorMessage = "Usernames must be between 3 and 20 characters", MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, ErrorMessage = "Password must be between 4 and 20 characters", MinimumLength = 4)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(50, ErrorMessage = "Email must not be longer than 50 characters")]
        public string Email { get; set; }

        public int RoleID { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; }

        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [StringLength(20, ErrorMessage = "Password must be between 4 and 20 characters", MinimumLength = 4)]
        public string ConfirmPassword { get; set; }

    }
}