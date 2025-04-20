using Gallery.Server.Infrastructure.Persistence.Configurations;
using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Infrastructure.Persistence.db
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
