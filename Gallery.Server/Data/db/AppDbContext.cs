using Gallery.Server.Data.Configurations;
using Gallery.Server.Models.Files.Image;
using Gallery.Server.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Data.db
{
    public class AppDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ImageModel> Images { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserModelConfiguration());
            modelBuilder.ApplyConfiguration(new ImageModelConfiguration());
        }
    }
}
