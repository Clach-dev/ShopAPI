using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder
            .HasKey(refreshToken => refreshToken.Token);

        builder
            .Property(refreshToken => refreshToken.ExpirationDate)
            .IsRequired(true);
        
        builder
            .HasOne(refreshToken => refreshToken.User)
            .WithOne(user => user.RefreshToken)
            .HasForeignKey<User>(user => user.RefreshTokenId);
    }
}