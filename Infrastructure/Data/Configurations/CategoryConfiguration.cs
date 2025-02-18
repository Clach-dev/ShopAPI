using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder
            .HasKey(category => category.Id);
        
        builder.
            Property(category => category.Name)
            .HasMaxLength(50)
            .IsRequired(true);
        
        builder
            .Property(category => category.Description)
            .HasMaxLength(300)
            .IsRequired(false);
        
        builder
            .HasMany(category => category.Products)
            .WithMany(product => product.Categories);
    }
}