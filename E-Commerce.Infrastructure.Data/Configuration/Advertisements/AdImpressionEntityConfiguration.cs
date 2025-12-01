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
    internal class AdImpressionEntityConfiguration : IEntityTypeConfiguration<AdImpression>
    {
        public void Configure(EntityTypeBuilder<AdImpression> builder)
        {
            builder.ToTable("AdImpressions");
            builder.HasKey(e => e.AdImpressionId);

            #region Foreignkeys
            builder.HasOne(e => e.Advertisement).WithMany(e => e.AdImpressions).HasForeignKey(f => f.AdvertisementId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
