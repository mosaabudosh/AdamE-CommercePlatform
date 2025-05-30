using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(e => e.RoleId);

            #region Seeding Data
            builder.HasData(new Role[]
            {
                new Role()
                { 
                    RoleId = (int)Roles.SuperAdmin,
                    Name = "Super Admin" , 
                    NameAr = "المشرف الأعلى",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Role()
                { 
                    RoleId = (int)Roles.Admin, 
                    Name = "Admin" , 
                    NameAr = "المشرف",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Role()
                { 
                    RoleId = (int)Roles.VendorAdmin, 
                    Name = "Vendors Admin" , 
                    NameAr = "مسؤول البائعين",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Role()
                { 
                    RoleId = (int)Roles.Vendor, 
                    Name = "Vendor" , 
                    NameAr = "بائع",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Role()
                { 
                    RoleId = (int)Roles.User, 
                    Name = "User" , 
                    NameAr = "مستخدم",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
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
