using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(e => e.OrderId);

            #region Foreignkeys
            builder.HasOne(e => e.OrderStatus).WithMany(e => e.Orders).HasForeignKey(f => f.OrderStatusId);
            builder.HasOne(e => e.Customer).WithMany(e => e.Orders).HasForeignKey(f => f.CustomerId);
            builder.HasOne(e => e.PaymentMethod).WithMany(e => e.Orders).HasForeignKey(f => f.PaymentMethodId);
            #endregion
        }
    }
}
