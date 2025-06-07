
using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
	public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole>
	{
		public void Configure(EntityTypeBuilder<UserRole> builder)
		{
			builder.ToTable("UserRoles");
			builder.HasKey(e => e.UserRoleId);

			#region Foreignkeys
			builder.HasOne(e => e.Role).WithMany(e => e.UserRoles).HasForeignKey(f => f.RoleId);
			builder.HasOne(e => e.User).WithMany(e => e.UserRoles).HasForeignKey(f => f.UserId);
            #endregion

            #region Seeding
            Guid userId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E");
            builder.HasData(new UserRole[]
            {
                 new UserRole(){
                    UserRoleId = (int)Roles.SuperAdmin,
                    UserId = userId ,
                    RoleId = (int)Roles.SuperAdmin,
                    IsActive = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = userId
                },
                new UserRole(){
                    UserRoleId = (int)Roles.Admin,
                    UserId = userId ,
                    RoleId = (int)Roles.Admin,
                    IsActive = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = userId
                },
                new UserRole(){
                    UserRoleId = (int)Roles.User,
                    UserId = userId ,
                    RoleId = (int)Roles.User,
                    IsActive = true,
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = userId
                }
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
