using FileMetadataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileMetadataService.Infrastructure.Data.Configurations;

public class FileActivityConfiguration : IEntityTypeConfiguration<FileActivity>
{
    public void Configure(EntityTypeBuilder<FileActivity> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileId)
            .IsRequired();

        builder.Property(a => a.ActivityType)
            .IsRequired();

        builder.Property(a => a.UserId)
            .IsRequired();

        builder.Property(a => a.Details)
            .HasMaxLength(500)
            .IsRequired();


        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Ignore domain events for EF Core
        builder.Ignore(a => a.DomainEvents);
    }
}