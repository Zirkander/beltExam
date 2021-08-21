using Microsoft.EntityFrameworkCore;

namespace beltExamActivity.Models
{
    public class BeltExamContext : DbContext
    {
        public BeltExamContext(DbContextOptions options) : base(options) { }

        // public DbSet<Post> Posts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<BeltExam> Activity { get; set; }
        public DbSet<ActivityUser> ActiveUser { get; set; }
    }
}