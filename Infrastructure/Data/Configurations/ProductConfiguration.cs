using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(product => product.Id);
        
        builder
            .Property(product => product.Name)
            .HasMaxLength(50)
            .IsRequired(true);
        
        builder
            .Property(product => product.Description)
            .HasMaxLength(300)
            .IsRequired(false);
        
        builder
            .Property(product => product.Price)
            .IsRequired(true);
        
        builder
            .Property(product => product.Amount)
            .IsRequired(true);

        builder
            .HasMany(product => product.Categories)
            .WithMany(category => category.Products);

        builder
            .HasMany(product => product.OrderItems)
            .WithOne(orderItem => orderItem.Product)
            .HasForeignKey(orderItem => orderItem.ProductId);
    }
}