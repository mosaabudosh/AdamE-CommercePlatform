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
                    Code = "DefaultTheme",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = PlatformTheme.NormalMode,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DefaultLanguage,
                    Name = "Default Language" ,
                    NameAr = "اللغة الأفتراضية",
                    Code = "DefaultLanguage",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = Convert.ToInt32(Enums.Languages.English).ToString(),
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Logo,
                    Name = "Logo" ,
                    NameAr = "الشعار",
                    Code = "Logo",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.LogoAr,
                    Name = "Logo Arabic" ,
                    NameAr = "شعار عربي",
                    Code = "LogoAr",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.WaterMark,
                    Name = "Water Mark" ,
                    NameAr = "العلامة المائية",
                    Code = "WaterMark",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
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
                    DataTypeId = (int)PlatformSettingsDataTypes.String
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
                    DataTypeId = (int)PlatformSettingsDataTypes.String
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
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.In24Format,
                    Name = "In24Format" ,
                    NameAr = "بتنسيق 24",
                    Code = "In24Format",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "true",
                    DataTypeId = (int)PlatformSettingsDataTypes.Boolean
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSUserPath,
                    Name = "DMS User Path" ,
                    NameAr = "مسار المستخدم في DMS",
                    Code = "DMSUserPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Users",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSSettingsPath,
                    Name = "DMS Settings Path" ,
                    NameAr = "مسار الإعدادات في DMS",
                    Code = "DMSSettingsPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Settings",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },

                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSRootFolderPath,
                    Name = "DMS Root Folder Path" ,
                    NameAr = "مسار المجلد الجذر لـ DMS",
                    Code = "DMSRootFolderPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "E:\\DMS\\",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSCategoryPath,
                    Name = "DMS Category Path" ,
                    NameAr = "مسار فئة DMS",
                    Code = "DMSCategoryPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Categories",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSCountryPath,
                    Name = "DMS Country Path" ,
                    NameAr = "مسار البلدان في DMS",
                    Code = "DMSCountryPath",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Countries",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DefaultPage,
                    Name = "Default Page" ,
                    NameAr = "الصفحة الافتراضية",
                    Code = "DefaultPage",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = ((int)Screens.Welcome).ToString(),
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DefaultMenuType,
                    Name = "Default Menu Type" ,
                    NameAr = "نوع القائمة الافتراضية",
                    Code = "DefaultMenuType",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = ((int)MenuType.Open).ToString(),
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
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
            builder.Ignore(p => p.IsActive);
            #endregion
        }
    }
}
