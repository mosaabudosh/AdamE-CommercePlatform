using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
{
    public class ActionEntityConfiguration : IEntityTypeConfiguration<Domain.Domains.Authorisation.Action>
    {
        public void Configure(EntityTypeBuilder<Domain.Domains.Authorisation.Action> builder)
        {
            builder.ToTable("Actions");
            builder.HasKey(e => e.ActionId);

			#region Seeding Data
			builder.HasData(new Domain.Domains.Authorisation.Action[]
            {
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Add, 
                    Name = "Add" , 
                    NameAr = "أضف",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Edit, 
                    Name = "Edit" , 
                    NameAr = "تعديل",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Delete, 
                    Name = "Delete" ,
                    NameAr = "حذف",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.View, 
                    Name = "View" , 
                    NameAr = "عرض",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Print, 
                    Name = "Print" , 
                    NameAr = "طباعة",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Active, 
                    Name = "Active" , 
                    NameAr = "فعال",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.InActive, 
                    Name = "InActive" , 
                    NameAr = "غير فعال",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Restor, 
                    Name = "Restor" , 
                    NameAr = "استعادة",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Download, 
                    Name = "Download" , 
                    NameAr = "تنزيل",
                    CreationDate = new DateTime(2025,1,1),
                    CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                    IsActive = true
                },
                new Domain.Domains.Authorisation.Action()
                { 
                    ActionId = (int)Actions.Upload, 
                    Name = "Upload" , 
                    NameAr = "تحميل",
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
