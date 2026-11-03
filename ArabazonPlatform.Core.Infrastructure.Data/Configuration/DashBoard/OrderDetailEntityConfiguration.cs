using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class OrderDetailEntityConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails");
            builder.HasKey(e => e.OrderDetailId);

            #region Foreignkeys
            builder.HasOne(e => e.Order).WithMany(e => e.OrderDetails).HasForeignKey(f => f.OrderId);
            builder.HasOne(e => e.ShopItem).WithMany(e => e.OrderDetails).HasForeignKey(f => f.ShopItemId);
            builder.HasOne(e => e.Color).WithMany(e => e.OrderDetails).HasForeignKey(f => f.ColorId);
            #endregion
        }
    }
}
