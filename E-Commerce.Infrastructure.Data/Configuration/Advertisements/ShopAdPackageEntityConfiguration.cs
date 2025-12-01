using E_Commerce.Domain.Domains.Advertisements;
using E_Commerce.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.Advertisements
{
    public class ShopAdPackageEntityConfiguration : IEntityTypeConfiguration<ShopAdPackage>
    {
        public void Configure(EntityTypeBuilder<ShopAdPackage> builder)
        {
            builder.ToTable("ShopAdPackages");
            builder.HasKey(e => e.ShopAdPackageId);

            #region Foreignkeys
            builder.HasOne(e => e.Shop).WithMany(e => e.ShopAdPackages).HasForeignKey(f => f.ShopId);
            builder.HasOne(e => e.AdPackage).WithMany(e => e.ShopAdPackages).HasForeignKey(f => f.AdPackageId);
            builder.HasOne(e => e.ShopAdPackageStatus).WithMany(e => e.ShopAdPackages).HasForeignKey(f => f.ShopAdPackageStatusId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
