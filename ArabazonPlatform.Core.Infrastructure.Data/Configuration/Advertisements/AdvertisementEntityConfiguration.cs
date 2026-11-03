using ArabazonPlatform.Domain.Domains.Advertisements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Advertisements
{
    public class AdvertisementEntityConfiguration : IEntityTypeConfiguration<Advertisement>
    {
        public void Configure(EntityTypeBuilder<Advertisement> builder)
        {
            builder.ToTable("Advertisements");
            builder.HasKey(e => e.AdvertisementId);

            #region Foreignkeys
            builder.HasOne(e => e.AdvertisementStatus).WithMany(e => e.Advertisements).HasForeignKey(f => f.AdvertisementStatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.ShopAdPackage).WithMany(e => e.Advertisements).HasForeignKey(f => f.ShopAdPackageId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(e => e.AdPackage).WithMany(e => e.Advertisements).HasForeignKey(f => f.AdPackageId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
