

using E_Commerce.Domain.Domains.Authorisation;
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
                    ScreenId = 1,
                    Name = "Users" ,
                    NameAr = "المستخدمين",
                    Url = "users",
                    Icon = "pi pi-users",
                    ModuleId = (int)Modules.AccssControl,
                    Order = 1,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    IsActive = true,
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E")
                },
                new Screen()
                {
                    ScreenId = 2,
                    Name = "Roles" ,
                    NameAr = "الأدوار",
                    Url = "roles",
                    Icon = "pi pi-briefcase",
                    ModuleId = (int)Modules.AccssControl,
                    Order = 2,
                    IsInMenue = true,
                    CreationDate = new DateTime(2025,1,1),
                    IsActive = true,
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E")
                },
            });
            #endregion
        }
    }
}
