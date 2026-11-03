using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;
using ArabazonPlatform.Domain.Enum;
using static ArabazonPlatform.Generic.Constants.Constants;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
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

            #region Seeding
            Guid userId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E");
            builder.HasData(new UserSetting[]
            {
                new UserSetting()
                {
                    UserSettingId = 1,
                    PlatformSettingId = (int)PlatformSettings.DefaultTheme,
                    Value = PlatformTheme.NormalMode,
                    UserId = userId ,
                    CreationUserId = userId,
                    CreationDate = new DateTime(2025,1,1),
                    IsActive = true
                },
                new UserSetting()
                {
                    UserSettingId = 2,
                    PlatformSettingId = (int)PlatformSettings.DefaultLanguage,
                    Value = Convert.ToInt32(Enums.Languages.English).ToString(),
                    UserId = userId ,
                    CreationUserId = userId,
                    CreationDate = new DateTime(2025,1,1),
                    IsActive = true
                },
                new UserSetting()
                {
                    UserSettingId = 3,
                    PlatformSettingId = (int)PlatformSettings.DefaultPage,
                    Value = Convert.ToInt32(Enums.Screens.Welcome).ToString(),
                    UserId = userId ,
                    CreationUserId = userId,
                    CreationDate = new DateTime(2025,1,1),
                    IsActive = true
                },
                new UserSetting()
                {
                    UserSettingId = 4,
                    PlatformSettingId = (int)PlatformSettings.DefaultMenuType,
                    Value = Convert.ToInt32(Enums.MenuType.Open).ToString(),
                    UserId = userId ,
                    CreationUserId = userId,
                    CreationDate = new DateTime(2025, 1, 1),
                    IsActive = true
                },
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
