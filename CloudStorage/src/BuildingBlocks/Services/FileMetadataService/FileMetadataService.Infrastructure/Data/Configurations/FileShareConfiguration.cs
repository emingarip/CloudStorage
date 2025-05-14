using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FileMetadataService.Infrastructure.Data.Configurations;

public class FileShareConfiguration : IEntityTypeConfiguration<Domain.Entities.FileShare>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.FileShare> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.FileId)
            .IsRequired();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Permission)
            .IsRequired();

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Ignore domain events for EF Core
        builder.Ignore(s => s.DomainEvents);
    }
}