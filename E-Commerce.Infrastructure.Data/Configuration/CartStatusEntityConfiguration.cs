

using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class CartStatusEntityConfiguration : IEntityTypeConfiguration<CartStatus>
	{
		public void Configure(EntityTypeBuilder<CartStatus> builder)
		{
			builder.ToTable("CartStatuses");
			builder.HasKey(e => e.CartStatusId);

			#region Seeding Data
			builder.HasData(new CartStatus[]
			{
				new CartStatus(){ CartStatusId = (int)CartStatuses.Enterd, Name = "Enterd" , NameAr = "أدخل"},
				new CartStatus(){ CartStatusId = (int)CartStatuses.Ordered, Name = "Ordered" , NameAr = "تم الطلب"},
				new CartStatus(){ CartStatusId = (int)CartStatuses.InProgrees, Name = "InProgrees" , NameAr = "قيد التنفيذ"},
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
