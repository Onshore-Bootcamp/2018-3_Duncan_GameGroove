using System.ComponentModel.DataAnnotations;

namespace GameGroove.Models
{
    public class GamePO
    {
        public int GameID { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, ErrorMessage = "Titles must be less than 100 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Release Date is required")]
        [StringLength(20, ErrorMessage = "Release date must be between 4 and 20 characters", MinimumLength = 4)]
        public string ReleaseDate { get; set; }

        [Required(ErrorMessage = "Developer is required")]
        [StringLength(50, ErrorMessage = "Developers must be less than 50 characters")]
        public string Developer { get; set; }

        [Required(ErrorMessage = "Platform is required")]
        [StringLength(50, ErrorMessage = "Platforms must be less than 50 characters")]
        public string Platform { get; set; }
    }
}