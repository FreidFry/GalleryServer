using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallery.Server.Infrastructure.Persistence.Configurations
{
    internal class UserModelConfiguration : IEntityTypeConfiguration<UserModel>
    {
        public void Configure(EntityTypeBuilder<UserModel> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.UserId);
            builder.Property(u => u.Username)
                   .IsRequired()
                   .HasMaxLength(50);
            builder.Property(u => u.PasswordHash)
                   .IsRequired();
            builder.Property(u => u.CreatedAt)
                   .IsRequired();
            builder.Property(u => u.LastLogin)
                   .IsRequired();

            builder.HasMany(u => u.Images)
                   .WithOne(i => i.User)
                   .HasForeignKey(i => i.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}