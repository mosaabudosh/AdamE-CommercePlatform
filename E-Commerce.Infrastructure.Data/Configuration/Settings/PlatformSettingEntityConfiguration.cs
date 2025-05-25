using Adam.Generic.Constants;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.Domains.Settings;
using E_Commerce.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Settings
{
    public class PlatformSettingEntityConfiguration : IEntityTypeConfiguration<PlatformSetting>
    {
        public void Configure(EntityTypeBuilder<PlatformSetting> builder)
        {
            builder.ToTable("PlatformSettings");
            builder.HasKey(e => e.PlatformSettingId);

            #region Foreignkeys
            builder.HasOne(e => e.DataType).WithMany(e => e.PlatformSettings).HasForeignKey(f => f.DataTypeId);
            #endregion

            #region Seeding Data
            builder.HasData(new PlatformSetting[]
            {
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DefaultTheme,
                    Name = "Default Theme" ,
                    NameAr = "الوضع الافتراضي",
                    Code = "Default Theme",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = PlatformTheme.NormalMode,
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DefaultLanguage,
                    Name = "Default Language" ,
                    NameAr = "اللغة الأفتراضية",
                    Code = "Default Language",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = Convert.ToInt32(Enums.Languages.English).ToString(),
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Logo,
                    Name = "Logo" ,
                    NameAr = "الشعار",
                    Code = "Logo",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.LogoAr,
                    Name = "Logo Arabic" ,
                    NameAr = "شعار عربي",
                    Code = "LogoAr",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.WaterMark,
                    Name = "Water Mark" ,
                    NameAr = "العلامة المائية",
                    Code = "WaterMark",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Name,
                    Name = "Name" ,
                    NameAr = "الأسم",
                    Code = "Name",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Adam E-Commerce Platform",
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.NameAr,
                    Name = "Name Arabic" ,
                    NameAr = "الأسم بالعربي",
                    Code = "NameAr",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "منصة آدم للتجارة الإلكترونية",
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DateFormat,
                    Name = "Date Format" ,
                    NameAr = "تنسيق التاريخ",
                    Code = "DateFormat",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "dd/MM/yyyy",
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.In24Format,
                    Name = "In24Format" ,
                    NameAr = "بتنسيق 24",
                    Code = "In24Format",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.Boolean
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSUserPath,
                    Name = "DMS User Path" ,
                    NameAr = "مسار المستخدم في DMS",
                    Code = "DMSUserPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSSettingsPath,
                    Name = "DMS Settings Path" ,
                    NameAr = "مسار الإعدادات في DMS",
                    Code = "DMSSettingsPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)DataTypes.String
                },
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.CreationDate);
            builder.Ignore(p => p.CreationUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            builder.Ignore(p => p.IsActive);
            #endregion
        }
    }
}
