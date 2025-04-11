using Gallery.Server.Models.User;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Server.Data.db
{
    public class UsersDbContext : DbContext
    {
        public DbSet<UserModel> Users { get; set; }

        public UsersDbContext(DbContextOptions<UsersDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>()
                .HasKey(u => u.UserId);
            modelBuilder.Entity<UserModel>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);
            modelBuilder.Entity<UserModel>()
                .Property(u => u.PasswordHash)
                .IsRequired();
            modelBuilder.Entity<UserModel>()
                .Property(u => u.CreatedAt)
                .IsRequired();
            modelBuilder.Entity<UserModel>()
                .Property(u => u.LastLogin)
                .IsRequired();
        }
    }
}
