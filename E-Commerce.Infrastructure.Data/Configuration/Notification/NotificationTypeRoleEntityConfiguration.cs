using E_Commerce.Domain.Domains;
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
    public class NotificationTypeRoleEntityConfiguration : IEntityTypeConfiguration<NotificationTypeRole>
    {
        public void Configure(EntityTypeBuilder<NotificationTypeRole> builder)
        {
            builder.ToTable("NotificationTypeRoles");
            builder.HasKey(e => e.NotificationTypeRoleId);

            #region Foreignkeys
            builder.HasOne(e => e.Role).WithMany(e => e.NotificationTypeRoles).HasForeignKey(f => f.RoleId);
            builder.HasOne(e => e.NotificationType).WithMany(e => e.NotificationTypeRoles).HasForeignKey(f => f.NotificationTypeId);
            #endregion

            #region Ignore
            builder.Ignore(p => p.DeletedDate);
            builder.Ignore(p => p.DeletedUserId);
            #endregion
        }
    }
}
