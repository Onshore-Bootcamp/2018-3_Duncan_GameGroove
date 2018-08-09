using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GameGroove.Models
{
    public class RequestPO
    {
        public int RequestID { get; set; }

        [Required(ErrorMessage = "Request is required")]
        [Display(Name = "Request")]
        [StringLength(500, ErrorMessage = "Requests must be between 10 and 500 characters", MinimumLength = 10)]
        public string RequestText { get; set; }
        
        public string Username { get; set; }
        
        public string Date { get; set; }
    }
}