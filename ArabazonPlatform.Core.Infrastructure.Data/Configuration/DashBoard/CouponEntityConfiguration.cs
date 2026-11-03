using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class CouponEntityConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupons");
            builder.HasKey(e => e.CouponId);

            #region Foreignkeys
            builder.HasOne(e => e.Shop).WithMany(e => e.Coupons).HasForeignKey(f => f.ShopId);
            #endregion
        }
    }
}
