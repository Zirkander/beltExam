using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace beltExamActivity.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Is required.")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Is requried.")]
        [MinLength(2, ErrorMessage = "Must be at least 2 characters.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Is required.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Is requried.")]
        [MinLength(8, ErrorMessage = "Must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Password is Invalid, Please make sure that there is at least 1 uppercase, 1 lowercase, and 1 special character.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [NotMapped] //Makes sure that it doesn't get added to DB
        [Required(ErrorMessage = "Is requried.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match!")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public List<BeltExam> CreatedActivities { get; set; }
        public List<ActivityUser> ActiveUser { get; set; }
    }
}