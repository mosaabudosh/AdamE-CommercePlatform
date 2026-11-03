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
    public class ShopCategoryEntityConfiguration : IEntityTypeConfiguration<ShopCategory>
    {
        public void Configure(EntityTypeBuilder<ShopCategory> builder)
        {
            builder.ToTable("ShopCategories");
            builder.HasKey(e => e.ShopCategoryId);

            #region Foreignkeys
            builder.HasOne(e => e.Shop).WithMany(e => e.ShopCategories).HasForeignKey(f => f.ShopId);
            builder.HasOne(e => e.Category).WithMany(e => e.ShopCategories).HasForeignKey(f => f.CategoryId);
            #endregion
        }
    }
}
