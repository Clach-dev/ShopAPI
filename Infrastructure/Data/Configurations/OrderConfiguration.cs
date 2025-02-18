using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .HasKey(order => order.Id);
        
        builder
            .Property(order => order.TotalPrice)
            .IsRequired(true);

        builder
            .Property(order => order.Status)
            .IsRequired(true);

        builder
            .Property(order => order.DeliveryDate)
            .IsRequired(true);

        builder
            .HasOne(order => order.User)
            .WithMany(user => user.Orders);
        
        builder
            .HasMany(order => order.OrderItems)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(orderItem => orderItem.OrderId);
    }
}