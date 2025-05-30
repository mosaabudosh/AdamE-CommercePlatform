using E_Commerce.Domain.Domains.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configuration.Notification
{
    public class NotificationEntityConfiguration : IEntityTypeConfiguration<E_Commerce.Domain.Domains.Notification.Notification>
    {
        public void Configure(EntityTypeBuilder<E_Commerce.Domain.Domains.Notification.Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(e => e.NotificationId);

            #region Foreign keys      
            builder.HasOne(i => i.NotificationStatus).WithMany(i => i.Notifications).HasForeignKey(k => k.NotificationStatusId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(i => i.NotificationType).WithMany(i => i.Notifications).HasForeignKey(k => k.NotificationTypeId).OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Ignore
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            builder.Ignore(x => x.LastUpdateUserId);
            builder.Ignore(x => x.LastUpdatedDate);
            #endregion
        }
    }
}
