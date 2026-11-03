using ArabazonPlatform.Domain.Domains.Notification;
using ArabazonPlatform.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Infrastructure.Data.Configuration.Notification
{
    public class NotificationStatusEntityConfiguration : IEntityTypeConfiguration<NotificationStatus>
    {
        public void Configure(EntityTypeBuilder<NotificationStatus> builder)
        {
            builder.ToTable("NotificationStatuses");
            builder.HasKey(e => e.NotificationStatusId);

            #region Seeding
            builder.HasData(new NotificationStatus[] 
            {
                 new NotificationStatus() 
                 { 
                     NotificationStatusId = (int)Enums.NotificationStatuses.Read, 
                     Name = "Read", 
                     NameAr = "تمت القراءة",
                     CreationDate = new DateTime(2025,1,1),
                     CreationUserId = new Guid("26435193-38F2-43B2-8841-3AB288E19F9E"),
                     IsActive = true
                 },
                 new NotificationStatus() 
                 { 
                     NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                     Name = "UnRead",
                     NameAr = "غير مقروء",
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
