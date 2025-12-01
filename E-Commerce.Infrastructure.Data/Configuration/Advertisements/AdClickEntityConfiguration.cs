using E_Commerce.Domain.Domains.Advertisements;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.Advertisements
{
    public class AdClickEntityConfiguration : IEntityTypeConfiguration<AdClick>
    {
        public void Configure(EntityTypeBuilder<AdClick> builder)
        {
            builder.ToTable("AdClicks");
            builder.HasKey(e => e.AdClickId);

            #region Foreignkeys
            builder.HasOne(e => e.Advertisement).WithMany(e => e.AdClicks).HasForeignKey(f => f.AdvertisementId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
