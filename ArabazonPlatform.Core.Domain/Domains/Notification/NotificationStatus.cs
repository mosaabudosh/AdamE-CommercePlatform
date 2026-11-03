using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Notification
{
    public class NotificationStatus : SharedBase
    {
        public int NotificationStatusId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<Notification> Notifications { set; get; }
    }
}
