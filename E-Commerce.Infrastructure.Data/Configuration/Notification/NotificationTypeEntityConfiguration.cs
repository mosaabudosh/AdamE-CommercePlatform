using E_Commerce.Domain.Domains.Notification;
using E_Commerce.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.Notification
{
    public class NotificationTypeEntityConfiguration : IEntityTypeConfiguration<NotificationType>
    {
        public void Configure(EntityTypeBuilder<NotificationType> builder)
        {
            builder.ToTable("NotificationTypes");
            builder.HasKey(e => e.NotificationTypeId);

            #region Seeding
            builder.HasData(new NotificationType[]
            {
                 new NotificationType()
                 {
                     NotificationTypeId = (int)Enums.NotificationTypes.CreateOrder,
                     Name = "Create Order",
                     NameAr = "إنشاء طلب",
                     IsPlaySound = true,
                     Code = Enums.NotificationTypes.CreateOrder.ToString(),
                     CreationDate = new DateTime(2025,1,1),
                     CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                     IsActive = true
                 },
                 new NotificationType()
                 {
                     NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                     Name = "Change Order Status",
                     NameAr = "تغيير حالة الطلب",
                     IsPlaySound = true,
                     Code = Enums.NotificationTypes.ChangeOrderStatus.ToString(),
                     CreationDate = new DateTime(2025,1,1),
                     CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                     IsActive = true
                 },
                  new NotificationType()
                 {
                     NotificationTypeId = (int)Enums.NotificationTypes.ChangeShopItemStatuses,
                     Name = "Change Shop Item Status",
                     NameAr = "تغيير حالة السلعة",
                     IsPlaySound = true,
                     Code = Enums.NotificationTypes.ChangeShopItemStatuses.ToString(),
                     CreationDate = new DateTime(2025,1,1),
                     CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                     IsActive = true
                 },
            });
            #endregion

            #region Ignore
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
