using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
{
    public class ModuleEntityConfiguration : IEntityTypeConfiguration<Domain.Domains.Authorisation.Module>
    {
        public void Configure(EntityTypeBuilder<Domain.Domains.Authorisation.Module> builder)
        {
            builder.ToTable("Modules");
            builder.HasKey(e => e.ModuleId);

            #region Seeding Data
            builder.HasData(new Domain.Domains.Authorisation.Module[]
            {
                new Domain.Domains.Authorisation.Module()
                { 
                    ModuleId = (int)Modules.AccssControl, 
                    Name = "Accss Controls" , 
                    NameAr = "التحكم في الوصول",
                    Order = 1,
                    IsInMenue = true,
                    Icon = "pi pi-shield"
                },
                new Domain.Domains.Authorisation.Module()
                { 
                    ModuleId = (int)Modules.Lookup,
                    Name = "Lookups" , 
                    NameAr = "البيانات",
                    Order = 2,
                    IsInMenue = true,
                    Icon = "pi pi-server"
                },
                new Domain.Domains.Authorisation.Module()
                { 
                    ModuleId = (int)Modules.Notification, 
                    Name = "Notifications" , 
                    NameAr = "الإشعارات",
                    Order = 3,
                    IsInMenue = true,
                    Icon = "pi pi-bell"
                },
                new Domain.Domains.Authorisation.Module()
                { 
                    ModuleId = (int)Modules.Report, 
                    Name = "Reports" , 
                    NameAr = "التقارير",
                    Order = 8, 
                    IsInMenue = true,
                    Icon = "pi pi-file"                    
                },
                new Domain.Domains.Authorisation.Module()
                {
                    ModuleId = (int)Modules.CustomerManagement,
                    Name = "Customer Management" ,
                    NameAr = "إدارة العملاء",
                    Order = 4,
                    IsInMenue = true,
                    Icon = "pi pi-briefcase"
                },
                new Domain.Domains.Authorisation.Module()
                {
                    ModuleId = (int)Modules.CRM,
                    Name = "CRM" ,
                    NameAr = "إدارة علاقات العملاء",
                    Order = 5,
                    IsInMenue = true,
                    Icon = "pi pi-sitemap"
                },
                new Domain.Domains.Authorisation.Module()
                {
                    ModuleId = (int)Modules.CMS,
                    Name = "CMS" ,
                    NameAr = "نظام إدارة المحتوى",
                    Order = 6,
                    IsInMenue = true,
                    Icon = "pi pi-cog"
                },
                new Domain.Domains.Authorisation.Module()
                {
                    ModuleId = (int)Modules.Advertisements,
                    Name = "Advertisements" ,
                    NameAr = "الإعلانات",
                    Order = 7,
                    IsInMenue = true,
                    Icon = "pi pi-flag-fill"
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
