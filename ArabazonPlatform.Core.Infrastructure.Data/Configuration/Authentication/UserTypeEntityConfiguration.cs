

using ArabazonPlatform.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Authorisation
{
	public class UserTypeEntityConfiguration : IEntityTypeConfiguration<UserType>
	{
		public void Configure(EntityTypeBuilder<UserType> builder)
		{
			builder.ToTable("UserTypes");
			builder.HasKey(e => e.UserTypeId);

			#region Seeding Data
			builder.HasData(new UserType[]
			{
				new UserType(){ UserTypeId = (int)UserTypes.OuerTeam, Name = "Ouer Team" , NameAr = "فريقنا"},
				new UserType(){ UserTypeId = (int)UserTypes.Vendor, Name = "Vendor" , NameAr = "بائع"},
				new UserType(){ UserTypeId = (int)UserTypes.Customer, Name = "Customer" , NameAr = "العميل"},
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
