using Gallery.Server.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallery.Server.Infrastructure.Persistence.Configurations
{
    internal class ImageModelConfiguration : IEntityTypeConfiguration<ImageModel>
    {
        public void Configure(EntityTypeBuilder<ImageModel> builder)
        {
            builder.ToTable("Images");

            builder.HasKey(i => i.ImageId);
            builder.Property(i => i.Name)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(i => i.RealImagePath)
                   .IsRequired();
            builder.Property(i => i.Description)
                   .IsRequired(false);
            builder.Property(i => i.Publicity)
                   .IsRequired();
            builder.Property(i => i.CreateAt)
                   .IsRequired();
        }
    }
}