using E_Commerce.Domain.Domains.Advertisements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Advertisements
{
    public class AdvertisementStatusEntityConfiguration : IEntityTypeConfiguration<AdvertisementStatus>
    {
        public void Configure(EntityTypeBuilder<AdvertisementStatus> builder)
        {
            builder.ToTable(" AdvertisementStatuses");
            builder.HasKey(e => e.AdvertisementStatusId);

            #region Seeding Data
            builder.HasData(new AdvertisementStatus[]
            {
                new AdvertisementStatus()
                { 
                    AdvertisementStatusId = (int)AdvertisementStatuses.Active, 
                    Name = "Active" , 
                    NameAr = "نشط", 
                    IsActive = true 
                },
                new AdvertisementStatus()
                {
                    AdvertisementStatusId = (int)AdvertisementStatuses.Paused,
                    Name = "Paused" ,
                    NameAr = "متوقف مؤقتًا",
                    IsActive = true
                },
                new AdvertisementStatus()
                {
                    AdvertisementStatusId = (int)AdvertisementStatuses.Expired,
                    Name = "Expired" ,
                    NameAr = "منتهية الصلاحية",
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
