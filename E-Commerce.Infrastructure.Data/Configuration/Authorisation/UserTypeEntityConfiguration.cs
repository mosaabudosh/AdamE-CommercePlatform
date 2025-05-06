

using E_Commerce.Domain.Domains.Authorisation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration.Authorisation
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
			builder.Ignore(p => p.CreationDate);
			builder.Ignore(p => p.CreationUserId);
			builder.Ignore(p => p.LastUpdatedDate);
			builder.Ignore(p => p.LastUpdateUserId);
			builder.Ignore(p => p.IsActive);
			#endregion
		}
	}
}
