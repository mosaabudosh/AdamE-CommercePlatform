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
                new Role(){ RoleId = (int)Roles.SuperAdmin, Name = "Super Admin" , NameAr = "المشرف الأعلى"},
                new Role(){ RoleId = (int)Roles.Admin, Name = "Admin" , NameAr = "المشرف "},
                new Role(){ RoleId = (int)Roles.VendorAdmin, Name = "Vendors Admin" , NameAr = "مسؤول البائعين"},
                new Role(){ RoleId = (int)Roles.Vendor, Name = "Vendor" , NameAr = "بائع"},
                new Role(){ RoleId = (int)Roles.User, Name = "User" , NameAr = "مستخدم"},
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
