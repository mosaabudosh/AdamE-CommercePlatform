

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
                    Url = "welcome-page",
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
                    Url = "chats-page",
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
                    Url = "settings-page",
                    Icon = "pi pi-cog",
                    ModuleId = (int)Modules.AccssControl,
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
