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
                    Order = 4, 
                    IsInMenue = true,
                    Icon = "pi pi-file"                    
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
