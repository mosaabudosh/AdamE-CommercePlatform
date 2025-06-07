

using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
	public class ScreenEntityConfiguration : IEntityTypeConfiguration<Screen>
	{
		public void Configure(EntityTypeBuilder<Screen> builder)
		{
			builder.ToTable("Screens");
			builder.HasKey(e => e.ScreenId);

			#region Foreignkeys
			builder.HasOne(e => e.Module).WithMany(e => e.Screens).HasForeignKey(f => f.ModuleId);
            #endregion

            #region Seeding Data
            builder.HasData(new Screen[]
            {
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Users,
                    Name = "Users" ,
                    NameAr = "المستخدمين",
                    Url = "users",
                    Icon = "pi pi-users",
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Roles,
                    Name = "Roles" ,
                    NameAr = "الأدوار",
                    Url = "roles",
                    Icon = "pi pi-briefcase",
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Action,
                    Name = "Actions" ,
                    NameAr = "الإجراءات",
                    Url = "actions",
                    Icon = "pi pi-sitemap",
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Screens,
                    Name = "Screens" ,
                    NameAr = "الصفحات",
                    Url = "screens",
                    Icon = "pi pi-file",
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Welcome,
                    Name = "Welcome" ,
                    NameAr = "مرحباً",
                    Url = "welcome",
                    Icon = string.Empty,
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = false,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Chats,
                    Name = "Chats" ,
                    NameAr = "الدردشات",
                    Url = "chats",
                    Icon = string.Empty,
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = false,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Settings,
                    Name = "Settings" ,
                    NameAr = "الإعدادات",
                    Url = "settings",
                    Icon = "pi pi-cog",
                    ModuleId = (int)Modules.AccssControl,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Cities,
                    Name = "Cities" ,
                    NameAr = "المدن",
                    Url = "cities",
                    Icon = "pi pi-map-marker",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Countries,
                    Name = "Countries" ,
                    NameAr = "البلدان",
                    Url = "countries",
                    Icon = "pi pi-map",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Items,
                    Name = "Items" ,
                    NameAr = "العناصر",
                    Url = "items",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Shops,
                    Name = "Shops" ,
                    NameAr = "المتاجر",
                    Url = "shops",
                    Icon = "pi pi-building",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Taxs,
                    Name = "Taxs" ,
                    NameAr = "الضرائب",
                    Url = "taxs",
                    Icon = "pi pi-asterisk",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Categories,
                    Name = "Categories" ,
                    NameAr = "التصنيفات",
                    Url = "categories",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Carts,
                    Name = "Carts" ,
                    NameAr = "العربات",
                    Url = "carts",
                    Icon = "pi pi-shopping-cart",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.CartStatuses,
                    Name = "Cart Statuses" ,
                    NameAr = "حالات عربة التسوق",
                    Url = "cart-statuses",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Orders,
                    Name = "Orders" ,
                    NameAr = "طلبات",
                    Url = "orders",
                    Icon = "pi pi-briefcase",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Reports,
                    Name = "Reports" ,
                    NameAr = "التقارير",
                    Url = "reports",
                    Icon = "pi pi-file",
                    ModuleId = (int)Modules.Report,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Notifications,
                    Name = "Notifications" ,
                    NameAr = "الإشعارات",
                    Url = "notifications",
                    Icon = "pi pi-bell",
                    ModuleId = (int)Modules.Notification,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.Discounts,
                    Name = "Discounts" ,
                    NameAr = "الخصومات",
                    Url = "discounts",
                    Icon = "pi pi-bell",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.NotificationStatuses,
                    Name = "Notification Statuses" ,
                    NameAr = "حالات الإشعارات",
                    Url = "notification-statuses",
                    Icon = "pi pi-bars",
                    ModuleId = (int)Modules.Notification,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.NotificationTypes,
                    Name = "Notification Types" ,
                    NameAr = "أنواع الإشعارات",
                    Url = "notification-types",
                    Icon = "pi pi-bars",
                    ModuleId = (int)Modules.Notification,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.ShopItems,
                    Name = "Shop Items" ,
                    NameAr = "مواد المتجر",
                    Url = "shop-items",
                    Icon = "pi pi-bars",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.OrderStatuses,
                    Name = "Order Statuses" ,
                    NameAr = "حالات الطلب",
                    Url = "order-statuses",
                    Icon = "pi pi-bars",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.PaymentMethod,
                    Name = "Payment Methods",
                    NameAr = "طرق الدفع",
                    Url = "payment-methods",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.UnitGroup,
                    Name = "Unit Groups",
                    NameAr = "مجموعة الوحدات",
                    Url = "unit-groups",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Screen()
                {
                    ScreenId = (int)Enums.Screens.ShopItemStatus,
                    Name = "Shop Item Statuses",
                    NameAr = "حالات السلعة في المتجر",
                    Url = "shop-item-statuses",
                    Icon = "pi pi-list",
                    ModuleId = (int)Modules.Lookup,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(p => p.LastUpdatedDate);
            builder.Ignore(p => p.LastUpdateUserId);
            #endregion
        }
    }
}
