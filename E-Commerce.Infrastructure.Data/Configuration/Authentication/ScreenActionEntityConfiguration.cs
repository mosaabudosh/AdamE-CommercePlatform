using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
    public class ScreenActionEntityConfiguration : IEntityTypeConfiguration<Domain.Domains.Authorisation.ScreenAction>
    {
        public void Configure(EntityTypeBuilder<Domain.Domains.Authorisation.ScreenAction> builder)
        {
            builder.ToTable("ScreenActions");
            builder.HasKey(e => e.ScreenActionId);

            #region Foreignkeys
            builder.HasOne(e => e.Screen).WithMany(e => e.ScreenActions).HasForeignKey(f => f.ScreenId);
            builder.HasOne(e => e.Action).WithMany(e => e.ScreenActions).HasForeignKey(f => f.ActionId);
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            builder.Ignore(p => p.IsActive);
            #endregion

        }
    }
}
