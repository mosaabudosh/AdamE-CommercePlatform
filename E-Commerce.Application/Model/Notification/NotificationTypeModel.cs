using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Notification
{
    public class NotificationTypeModel
    {
        public int NotificationTypeId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Icon { get; set; }
        public int Code { get; set; }
        public bool IsPlaySound { get; set; }
    }
}
