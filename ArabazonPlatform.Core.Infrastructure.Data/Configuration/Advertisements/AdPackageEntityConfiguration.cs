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
    public class AdPackageEntityConfiguration : IEntityTypeConfiguration<AdPackage>
    {
        public void Configure(EntityTypeBuilder<AdPackage> builder)
        {
            builder.ToTable("AdPackages");
            builder.HasKey(e => e.AdPackageId);

            #region Foreignkeys
            builder.HasOne(e => e.AdType).WithMany(e => e.AdPackages).HasForeignKey(f => f.AdTypeId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
