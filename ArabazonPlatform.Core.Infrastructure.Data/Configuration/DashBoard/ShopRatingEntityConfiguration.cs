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
    public class ShopRatingEntityConfiguration : IEntityTypeConfiguration<ShopRating>
    {
        public void Configure(EntityTypeBuilder<ShopRating> builder)
        {
            builder.ToTable("ShopRatings");
            builder.HasKey(e => e.ShopRatingId);

            #region Foreignkeys
            builder.HasOne(e => e.Shop).WithMany(e => e.ShopRatings).HasForeignKey(f => f.ShopId);
            builder.HasOne(e => e.Customer).WithMany(e => e.ShopRatings).HasForeignKey(f => f.CustomerId);
            #endregion
        }
    }
}
