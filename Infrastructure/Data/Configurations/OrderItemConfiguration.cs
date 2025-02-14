using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .HasKey(orderItem => orderItem.Id);

        builder
            .Property(orderItem => orderItem.Amount)
            .IsRequired(true);
        
        builder
            .HasOne(orderItem => orderItem.Order)
            .WithMany(order => order.OrderItems);
        
        builder
            .HasOne(orderItem => orderItem.Product)
            .WithMany(product => product.OrderItems);
    }
}