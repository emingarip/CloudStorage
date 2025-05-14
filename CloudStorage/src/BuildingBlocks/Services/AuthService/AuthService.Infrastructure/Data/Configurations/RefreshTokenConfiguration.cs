using AuthService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);

        builder.Property(rt => rt.Token)
            .IsRequired();

        builder.Property(rt => rt.UserId)
            .IsRequired();

        builder.Property(rt => rt.ExpiryDate)
            .IsRequired();

        builder.Property(rt => rt.CreatedAt)
            .IsRequired();

        builder.Property(rt => rt.CreatedByIp)
            .IsRequired();

        builder.Property(rt => rt.RevokedByIp)
            .IsRequired(false);

        builder.Property(rt => rt.ReasonRevoked)
                        .IsRequired(false);

        builder.Property(rt => rt.RevokedAt)
                        .IsRequired(false);

        // Create index for faster token lookups
        builder.HasIndex(rt => rt.Token);

        // Create index for faster user-based lookups
        builder.HasIndex(rt => rt.UserId);
    }
}