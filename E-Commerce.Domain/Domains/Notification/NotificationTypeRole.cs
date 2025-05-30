using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Notification
{
    public class NotificationTypeRole : SharedBase
    {
        public int NotificationTypeRoleId { get; set; }
        public int NotificationTypeId { get; set; }
        public int RoleId { get; set; }

        public virtual NotificationType NotificationType { set; get; }
        public virtual Role Role { set; get; }
    }
}
