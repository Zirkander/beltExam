using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace beltExamActivity.Models
{
    public class BeltExam
    {
        [Key]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Is required!")]
        [MinLength(3, ErrorMessage = "Must be greater than 3!")]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; }

        [Required(ErrorMessage = "Is required!")]
        [Display(Name = "Duration")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Is required!")]
        [DataType(DataType.Date)]
        [Display(Name = "Activity Date")]
        public DateTime ActivityDate { get; set; }

        [Required(ErrorMessage = "Is required!")]
        [DataType(DataType.Time)]
        [Display(Name = "Activity Time")]
        public DateTime ActivityTime { get; set; }

        [Required(ErrorMessage = "Is required!")]
        [MinLength(5, ErrorMessage = "Must be greater than 5 characters!")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User User { get; set; }
        public List<ActivityUser> ActiveUser { get; set; }

    }
}