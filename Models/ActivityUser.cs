using System;
using System.ComponentModel.DataAnnotations;

namespace beltExamActivity.Models
{
    public class ActivityUser
    {
        [Key]
        public int ActivityUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int UserId { get; set; }
        public User User { get; set; }
        public int ActivityId { get; set; }
        public BeltExam Activity { get; set; }
    }
}