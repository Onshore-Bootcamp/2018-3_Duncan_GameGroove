using System.ComponentModel.DataAnnotations;

namespace GameGroove.Models
{
    public class ReviewPO
    {
        public int ReviewID { get; set; }

        [Required(ErrorMessage = "Review is required")]
        [Display(Name = "Review")]
        [StringLength(1000, ErrorMessage = "Reviews must be less than 1000 characters")]
        public string ReviewText { get; set; }

        [Display(Name = "Date Posted")]
        public string DatePosted { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(30, ErrorMessage = "Categories must be between 3 and 30 characters", MinimumLength = 3)]
        public string Category { get; set; }
        
        public int UserID { get; set; }

        public int GameID { get; set; }

        [Display(Name = "Title")]
        public string GameTitle { get; set; }

        public string Username { get; set; }
    }
}