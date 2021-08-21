using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace beltExamActivity.Models
{
    [NotMapped]
    public class LoginUser
    {
        [Required(ErrorMessage = "Is required.")]
        [EmailAddress]
        [Display(Name = "LoginEmail")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "Is requried.")]
        [MinLength(8, ErrorMessage = "Must be at least 8 characters.")]
        [Display(Name = "LoginPassword")]
        public string LoginPassword { get; set; }
    }
}