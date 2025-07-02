using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configuration.DashBoard
{
    public class ShopItemEntityConfiguration : IEntityTypeConfiguration<ShopItem>
    {
        public void Configure(EntityTypeBuilder<ShopItem> builder)
        {
            builder.ToTable("ShopItems");
            builder.HasKey(e => e.ShopItemId);

            #region Foreignkeys
            builder.HasOne(e => e.Tax).WithMany(e => e.ShopItems).HasForeignKey(f => f.TaxId);
            builder.HasOne(e => e.Item).WithMany(e => e.ShopItems).HasForeignKey(f => f.ItemId);
            builder.HasOne(e => e.UnitGroup).WithMany(e => e.ShopItems).HasForeignKey(f => f.UnitGroupId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ShopItemStatus).WithMany(e => e.ShopItems).HasForeignKey(f => f.ShopItemStatusId).OnDelete(DeleteBehavior.Restrict);
            #endregion
        }
    }
}
