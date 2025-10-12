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
            builder.HasOne(e => e.UnitGroup).WithMany(e => e.ShopItems).HasForeignKey(f => f.UnitGroupId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ShopItemStatus).WithMany(e => e.ShopItems).HasForeignKey(f => f.ShopItemStatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ColorGroup).WithMany(e => e.ShopItems).HasForeignKey(f => f.ColorGroupId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Discount).WithMany(e => e.ShopItems).HasForeignKey(f => f.DiscountId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Color).WithMany(e => e.ShopItems).HasForeignKey(f => f.DefaultColorId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Unit).WithMany(e => e.ShopItems).HasForeignKey(f => f.DefaultUnitId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.Category).WithMany(e => e.ShopItems).HasForeignKey(f => f.CategoryId);
            builder.HasOne(e => e.ShopMenu).WithMany(e => e.ShopItems).HasForeignKey(f => f.ShopMenuId);
            builder.HasOne(e => e.Brand).WithMany(e => e.ShopItems).HasForeignKey(f => f.BrandId);
            #endregion
        }
    }
}
