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
        public string Subject { get; set; }
        public string? SubjectAr { get; set; }
        public string? Description { get; set; }
        public string? DescriptionAr { get; set; }
        public int UserId { get; set; }
        public int AlertStatusId { get; set; }
        public int AlertTypeId { get; set; }
        public string? AlertCode { get; set; }
        public string? Tooltip { get; set; }
        public string? TooltipAr { get; set; }
        public int NotificationStatusId { get; set; }
        public int NotificationTypeId { get; set; }

        public virtual NotificationStatus NotificationStatus { get; set; }
        public virtual NotificationType NotificationType { get; set; }
    }
}
