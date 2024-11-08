using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ServerGame106.Models;
namespace ServerGame106.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<GameLevel> GameLevels { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<LevelResult> LevelResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<GameLevel>().HasData(
                new GameLevel { LevelID = 1, title = "level 1"},
                new GameLevel { LevelID = 2, title = "level 2"},
                new GameLevel { LevelID = 3, title = "level 3"}
            );
            modelBuilder.Entity<Region>().HasData(
                new Region { RegionId = 1, RegionName = "dong bang song hong"},
                new Region { RegionId =2, RegionName = "dong bang song cuu long"}
            );
            modelBuilder.Entity<Question>().HasData(
                new Question
                {
                    QuestionId = 1,
                    ContentQuestion = "cau hoi 1",
                    Answer = "dap an 1",
                    Option1 = "dap an 1",
                    Option2 = "dap an 2",
                    Option3 = "dap an 3",
                    Option4 = "dap an 4",
                    LevelId = 1
                },
                new Question
                {
                    QuestionId = 2,
                    ContentQuestion = "cau hoi 2",
                    Answer = "dap an 2",
                    Option1 = "dap an 1",
                    Option2 = "dap an 2",
                    Option3 = "dap an 3",
                    Option4 = "dap an 4",
                    LevelId = 2
                }
            );
        }
    }
    
}
    