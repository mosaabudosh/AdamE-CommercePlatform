using ArabazonPlatform.Domain.Domains.Advertisements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Advertisements
{
    public class ShopAdPackageStatusEntityConfiguration : IEntityTypeConfiguration<ShopAdPackageStatus>
    {
        public void Configure(EntityTypeBuilder<ShopAdPackageStatus> builder)
        {
            builder.ToTable(" ShopAdPackageStatuses");
            builder.HasKey(e => e.ShopAdPackageStatusId);

            #region Seeding Data
            builder.HasData(new ShopAdPackageStatus[]
            {
                new ShopAdPackageStatus()
                {
                    ShopAdPackageStatusId = (int)ShopAdPackageStatuses.Active,
                    Name = "Active" ,
                    NameAr = "نشط",
                    IsActive = true
                },
                new ShopAdPackageStatus()
                {
                    ShopAdPackageStatusId = (int)ShopAdPackageStatuses.Expired,
                    Name = "Expired" ,
                    NameAr = "منتهية الصلاحية",
                    IsActive = true
                },
                new ShopAdPackageStatus()
                {
                    ShopAdPackageStatusId = (int)ShopAdPackageStatuses.Pending,
                    Name = "Pending" ,
                    NameAr = "معلق",
                    IsActive = true
                },
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            #endregion
        }
    }
}
