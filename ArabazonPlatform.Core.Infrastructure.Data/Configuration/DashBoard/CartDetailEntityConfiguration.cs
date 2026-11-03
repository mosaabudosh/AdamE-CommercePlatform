using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class CartDetailEntityConfiguration : IEntityTypeConfiguration<CartDetail>
    {
        public void Configure(EntityTypeBuilder<CartDetail> builder)
        {
            builder.ToTable("CartDetails");
            builder.HasKey(e => e.CartDetailId);

            #region Foreignkeys
            builder.HasOne(e => e.ShopItem).WithMany(e => e.CartDetails).HasForeignKey(f => f.ShopItemId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
