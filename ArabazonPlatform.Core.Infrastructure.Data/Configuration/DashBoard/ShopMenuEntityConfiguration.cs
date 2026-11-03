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
    internal class ShopMenuEntityConfiguration : IEntityTypeConfiguration<ShopMenu>
    {
        public void Configure(EntityTypeBuilder<ShopMenu> builder)
        {
            builder.ToTable("ShopMenus");
            builder.HasKey(e => e.ShopMenuId);

            #region Foreignkeys
            builder.HasOne(e => e.Shop).WithMany(e => e.ShopMenus).HasForeignKey(f => f.ShopId);
            #endregion
        }
    }
}
