using FileMetadataService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileMetadataService.Infrastructure.Data.Configurations;

public class FileVersionConfiguration : IEntityTypeConfiguration<FileVersion>
{
    public void Configure(EntityTypeBuilder<FileVersion> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.FileId)
            .IsRequired();

        builder.Property(v => v.Path)
            .IsRequired();

        builder.Property(v => v.Size)
            .IsRequired();

        builder.Property(v => v.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        // Ignore domain events for EF Core
        builder.Ignore(v => v.DomainEvents);
    }
}