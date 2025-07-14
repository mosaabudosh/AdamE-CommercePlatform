using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Notification
{
    public class Notification : SharedBase
    {
        public Guid NotificationId { get; set; }
        public string? Subject { get; set; }
        public string? SubjectAr { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public Guid? UserId { get; set; }
        public Guid? CustomerId { get; set; }
        public string? NotificationCode { get; set; }
        public string? Tooltip { get; set; }
        public string? TooltipAr { get; set; }
        public int NotificationStatusId { get; set; }
        public int NotificationTypeId { get; set; }

        public virtual NotificationStatus NotificationStatus { get; set; }
        public virtual NotificationType NotificationType { get; set; }
        public virtual User User { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
