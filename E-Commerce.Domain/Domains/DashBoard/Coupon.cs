using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Coupon : SharedBase
    {
        public int CouponId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxNumberOfUsage { get; set; }
        public int? ShopId { get; set; }

        public virtual Shop Shop { get; set; }

    }
}
