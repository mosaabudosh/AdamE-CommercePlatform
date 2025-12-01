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
                    Group = "Platform",
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
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = Convert.ToInt32(Enums.Languages.English).ToString(),
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Logo,
                    Name = "Logo" ,
                    NameAr = "الشعار",
                    Code = "Logo",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.File
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.LogoAr,
                    Name = "Logo Arabic" ,
                    NameAr = "شعار عربي",
                    Code = "LogoAr",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.File
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.WaterMark,
                    Name = "Water Mark" ,
                    NameAr = "العلامة المائية",
                    Code = "WaterMark",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.File
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Name,
                    Name = "Name" ,
                    NameAr = "الأسم",
                    Code = "Name",
                    Group = "Platform",
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
                    Group = "Platform",
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
                    Group = "Platform",
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
                    Group = "Platform",
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
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Users",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSRootFolderPath,
                    Name = "DMS Root Folder Path" ,
                    NameAr = "مسار المجلد الجذر لـ DMS",
                    Code = "DMSRootFolderPath",
                    Group = "DMS",
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
                    Group = "DMS",
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
                    Group = "DMS",
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
                    Group = "DMS",
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
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = ((int)MenuType.Open).ToString(),
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.LicenceNumber,
                    Name = "Licence Number" ,
                    NameAr = "رقم الرخصة",
                    Code = "LicenceNumber",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "XXXXXXXXXX",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.TaxNumber,
                    Name = "Tax Number" ,
                    NameAr = "الرقم الضريبي",
                    Code = "TaxNumber",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "XXXXXXXXXX",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.CRNumber,
                    Name = "Commercial Registration Number" ,
                    NameAr = "رقم السجل التجاري",
                    Code = "CRNumber",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "XXXXXXXXXX",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Address,
                    Name = "Address" ,
                    NameAr = "العنوان",
                    Code = "Address",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Amman",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.AddressAr,
                    Name = "Address In Arabic" ,
                    NameAr = "العنوان باللغة العربية",
                    Code = "AddressAr",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "عمان",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.MobileNumber,
                    Name = "Mobile Number" ,
                    NameAr = "رقم الجوال",
                    Code = "MobileNumber",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "+962788647816",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Website,
                    Name = "Website" ,
                    NameAr = "الموقع الإلكتروني",
                    Code = "Website",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "www.AdamE-CommercePlatform.com",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.Email,
                    Name = "Email" ,
                    NameAr = "البريد الإلكتروني",
                    Code = "Email",
                    Group = "Platform",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "AdamE-CommercePlatform@gmail.com",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSCustomerPath,
                    Name = "DMS Customer Path" ,
                    NameAr = "مسار عميل DMS",
                    Code = "DMSCustomerPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Customers",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSItemPath,
                    Name = "DMS Item Path" ,
                    NameAr = "مسار عنصر DMS",
                    Code = "DMSItemPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Items",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSPaymentMethodPath,
                    Name = "DMS Payment Method Path" ,
                    NameAr = "مسار طرق الدفع DMS",
                    Code = "DMSPaymentMethodPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "PaymentMethods",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSShopItemPath,
                    Name = "DMS Shop Item Path" ,
                    NameAr = "مسار عنصر متجر DMS",
                    Code = "DMSShopItemPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "ShopItems",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSShopPath,
                    Name = "DMS Shop Path" ,
                    NameAr = "مسار متجر DMS",
                    Code = "DMSShopPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "Shops",
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.NumberOfFeaturedItemsInDashBoared,
                    Name = "Number Of Featured Items In DashBoared" ,
                    NameAr = "عدد العناصر المميزة في لوحة التحكم",
                    Code = "NumberOfFeaturedItemsInDashBoared",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "20",
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.NumberOfCategoriesInDashBoared,
                    Name = "Number Of Categories In DashBoared" ,
                    NameAr = "عدد الفئات في لوحة المعلومات",
                    Code = "NumberOfCategoriesInDashBoared",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    Value = "20",
                    DataTypeId = (int)PlatformSettingsDataTypes.Integer
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.GooglePlayUrl,
                    Name = "Google Play Url" ,
                    NameAr = "رابط متجر جوجل بلاي",
                    Code = "GooglePlayUrl",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.AppStoreUrl,
                    Name = "App Store Url" ,
                    NameAr = "رابط متجر التطبيقات",
                    Code = "AppStoreUrl",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.FacebookUrl,
                    Name = "Facebook Url" ,
                    NameAr = "رابط الفيسبوك",
                    Code = "FacebookUrl",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.InstagramUrl,
                    Name = "Instagram Url" ,
                    NameAr = "رابط الانستغرام",
                    Code = "InstagramUrl",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.SnapchatUrl,
                    Name = "Snapchat Url" ,
                    NameAr = "رابط سناب شات",
                    Code = "SnapchatUrl",
                    Group = "DashBoared",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSBrandPath,
                    Name = "DMS Brand Path" ,
                    NameAr = "مسار العلامات التجارية DMS",
                    Code = "DMSBrandPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
                },
                new PlatformSetting()
                {
                    PlatformSettingId = (int)PlatformSettings.DMSAdPath,
                    Name = "DMS Ad Path" ,
                    NameAr = "مسار إعلان DMS",
                    Code = "DMSAdPath",
                    Group = "DMS",
                    IsReadOnly = false,
                    IsVisible = true,
                    DataTypeId = (int)PlatformSettingsDataTypes.String
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
