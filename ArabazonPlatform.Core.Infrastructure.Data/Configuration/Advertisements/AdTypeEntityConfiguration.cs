using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.Domains.DashBoard;
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
    public class AdTypeEntityConfiguration : IEntityTypeConfiguration<AdType>
    {
        public void Configure(EntityTypeBuilder<AdType> builder)
        {
            builder.ToTable(" AdTypes");
            builder.HasKey(e => e.AdTypeId);

            #region Seeding Data
            builder.HasData(new AdType[]
            {
                new  AdType(){ AdTypeId = (int)AdTypes.Banner, Name = "Banner" , NameAr = "بانر", IsActive = true },
                new  AdType(){ AdTypeId = (int)AdTypes.Video, Name = "Video" , NameAr = "فيديو", IsActive = true },
                new  AdType(){ AdTypeId = (int)AdTypes.InDoorAd, Name = "In DoorAd" , NameAr = "إعلان داخلي", IsActive = true },
                new  AdType(){ AdTypeId = (int)AdTypes.OutDoorAd, Name = "Out DoorAd" , NameAr = "إعلان خارجي", IsActive = true},
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
