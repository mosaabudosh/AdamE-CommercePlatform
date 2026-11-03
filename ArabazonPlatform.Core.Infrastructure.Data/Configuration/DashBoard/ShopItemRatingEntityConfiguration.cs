using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class ShopItemRatingEntityConfiguration : IEntityTypeConfiguration<ShopItemRating>
    {
        public void Configure(EntityTypeBuilder<ShopItemRating> builder)
        {
            builder.ToTable("ShopItemRatings");
            builder.HasKey(e => e.ShopItemRatingId);

            #region Foreignkeys
            builder.HasOne(e => e.ShopItem).WithMany(e => e.ShopItemRatings).HasForeignKey(f => f.ShopItemId);
            builder.HasOne(e => e.Customer).WithMany(e => e.ShopItemRatings).HasForeignKey(f => f.CustomerId);
            #endregion
        }
    }
}
