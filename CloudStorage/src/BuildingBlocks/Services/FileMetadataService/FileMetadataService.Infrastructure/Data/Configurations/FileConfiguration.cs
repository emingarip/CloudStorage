using FileMetadataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileMetadataService.Infrastructure.Data.Configurations;

public class FileConfiguration : IEntityTypeConfiguration<FileEntity>
{
    public void Configure(EntityTypeBuilder<FileEntity> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(f => f.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.Size)
            .IsRequired();

        builder.Property(f => f.Path)
            .IsRequired();

        builder.Property(f => f.OwnerId)
            .IsRequired();

        builder.Property(f => f.Status)
            .IsRequired();

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.Property(f => f.CreatedAt)
            .IsRequired();

        builder.Property(f => f.UpdatedAt);

        // Relationships
        builder.HasMany(f => f.Shares)
            .WithOne()
            .HasForeignKey(s => s.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Versions)
            .WithOne()
            .HasForeignKey(v => v.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(f => f.Activities)
            .WithOne()
            .HasForeignKey(a => a.FileId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF Core
        builder.Ignore(f => f.DomainEvents);
    }
}