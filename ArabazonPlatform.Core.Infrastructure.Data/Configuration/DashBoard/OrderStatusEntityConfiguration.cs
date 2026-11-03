using ArabazonPlatform.Domain.Domains.DashBoard;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.DashBoard
{
    public class OrderStatusEntityConfiguration : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> builder)
        {
            builder.ToTable("OrderStatuses");
            builder.HasKey(e => e.OrderStatusId);

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
                new OrderStatus(){ OrderStatusId = (int)OrderStatuses.Refund, Name = "Refund" , NameAr = "استرداد"},
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
