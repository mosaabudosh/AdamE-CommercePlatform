using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class ShopEntityConfiguration : IEntityTypeConfiguration<Shop>
    {
        public void Configure(EntityTypeBuilder<Shop> builder)
        {
            builder.ToTable("Shops");
            builder.HasKey(e => e.ShopId);

            #region Foreignkeys
            builder.HasOne(e => e.City).WithMany(e => e.Shops).HasForeignKey(f => f.CityId);
            builder.HasOne(e => e.ParentShop).WithMany(e => e.ChildShops).HasForeignKey(f => f.ParentShopId);
            #endregion
        }
    }
}
