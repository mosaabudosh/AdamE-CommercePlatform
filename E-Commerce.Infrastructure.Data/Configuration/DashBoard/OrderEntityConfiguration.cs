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
            builder.HasOne(e => e.CustomerCardInfo).WithMany(e => e.Orders).HasForeignKey(f => f.CustomerCardInfoId);

            builder.HasOne(e => e.CreationUser).WithMany(e => e.CreationUserOrders).HasForeignKey(f => f.CreationUserId);
            builder.HasOne(e => e.LastUpdateUser).WithMany(e => e.LastUpdateUsersOrders).HasForeignKey(f => f.LastUpdateUserId);
            builder.HasOne(e => e.DeletedUser).WithMany(e => e.DeletedUsersOrders).HasForeignKey(f => f.DeletedUserId);
            #endregion

            #region Unique
            builder.HasIndex(p => p.ReferenceNumber).IsUnique();
            #endregion
        }
    }
}
