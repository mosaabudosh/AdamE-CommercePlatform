

using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
{
    public class RoleScreenEntityConfiguration : IEntityTypeConfiguration<RoleScreen>
	{
		public void Configure(EntityTypeBuilder<RoleScreen> builder)
		{
			builder.ToTable("RoleScreens");
			builder.HasKey(e => e.RoleScreenId);

			#region Foreignkeys
			builder.HasOne(e => e.Role).WithMany(e => e.RoleScreens).HasForeignKey(f => f.RoleId);
			builder.HasOne(e => e.Screen).WithMany(e => e.RoleScreens).HasForeignKey(f => f.ScreenId);
            #endregion

            #region Unique
            builder.HasIndex(p => new { p.RoleId ,p.ScreenId}).IsUnique();
            #endregion

            #region Seeding Data
            builder.HasData(new RoleScreen[]
			{
				new RoleScreen()
				{
                    RoleScreenId = 1,
					ScreenId = 1,
					RoleId = (int)Roles.SuperAdmin,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new RoleScreen()
                {
                    RoleScreenId = 2,
                    ScreenId = 2,
                    RoleId = (int)Roles.SuperAdmin,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                }
            });
            #endregion

            #region NotMapped
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion

        }
    }
}
