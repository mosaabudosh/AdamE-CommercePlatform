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
    public class NotificationTypeEntityConfiguration : IEntityTypeConfiguration<NotificationType>
    {
        public void Configure(EntityTypeBuilder<NotificationType> builder)
        {
            builder.ToTable("NotificationTypes");
            builder.HasKey(e => e.NotificationTypeId);

            #region Ignore
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
