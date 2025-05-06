
using E_Commerce.Domain.Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Infrastructure.Data.Configuration
{
	public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
	{
		public void Configure(EntityTypeBuilder<OrderStatus> builder)
		{
			builder.ToTable("OrderStatuses");
			builder.HasKey(e => e.OrderStatusId);
            builder.Property(e => e.OrderStatusId).ValueGeneratedNever();



            #region Seeding Data
            builder.HasData(new OrderStatus[]
			{
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Entered, Name = "Entered" , NameAr = "تم إدخاله"},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Approved, Name = "Approved" , NameAr = "تمت القبول"},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.InProgrees, Name = "In Progrees" , NameAr = "قيد التجهيز"},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.InDelivery, Name = "In Delivery" , NameAr = "في التوصيل"},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Delivered, Name = "Delivered" , NameAr = "تم التوصيل "},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Canceled, Name = "Canceled" , NameAr = "ملغي"},
				new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Rejected, Name = "Rejected" , NameAr = "مرفوض"},
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
