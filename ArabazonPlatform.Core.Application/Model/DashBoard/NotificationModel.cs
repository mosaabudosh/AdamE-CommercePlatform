using ArabazonPlatform.Domain.Domains.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class NotificationModel
    {
        public Guid? NotificationId { get; set; }
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
        public string? NotificationStatus { get; set; }
        public string? NotificationType { get; set; }
        public string? Customer { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
