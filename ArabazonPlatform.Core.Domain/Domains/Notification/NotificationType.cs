using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Notification
{
    public class NotificationType : SharedBase
    {
        public int NotificationTypeId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Icon { get; set; }
        public string Code { get; set; }
        public bool IsPlaySound { get; set; }

        public virtual List<Notification> Notifications { set; get; }
        public virtual List<NotificationTypeRole> NotificationTypeRoles { set; get; }

    }
}
