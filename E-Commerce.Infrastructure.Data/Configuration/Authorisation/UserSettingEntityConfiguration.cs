using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
    public class UserSettingEntityConfiguration : IEntityTypeConfiguration<UserSetting>
    {
        public void Configure(EntityTypeBuilder<UserSetting> builder)
        {
            builder.ToTable("UserSettings");
            builder.HasKey(e => e.UserSettingId);

            #region Foreignkeys
            builder.HasOne(e => e.User).WithMany(e => e.UserSettings).HasForeignKey(f => f.UserId);
            builder.HasOne(e => e.PlatformSetting).WithMany(e => e.UserSettings).HasForeignKey(f => f.PlatformSettingId);
            #endregion
        }
    }
}
