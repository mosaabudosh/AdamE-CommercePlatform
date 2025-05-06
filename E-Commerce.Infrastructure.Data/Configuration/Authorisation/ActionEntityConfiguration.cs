using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
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
                new Domain.Domains.Authorisation.Action(){ ActionId = (int)Actions.Add, Name = "Add" , NameAr = "أضف"},
                new Domain.Domains.Authorisation.Action(){ ActionId = (int)Actions.Edit, Name = "Edit" , NameAr = "تعديل"},
                new Domain.Domains.Authorisation.Action(){ ActionId = (int)Actions.Delete, Name = "Delete" , NameAr = "حذف"},
                new Domain.Domains.Authorisation.Action(){ ActionId = (int)Actions.View, Name = "View" , NameAr = "عرض"},
                new Domain.Domains.Authorisation.Action(){ ActionId = (int)Actions.Print, Name = "Print" , NameAr = "طباعة"},
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
