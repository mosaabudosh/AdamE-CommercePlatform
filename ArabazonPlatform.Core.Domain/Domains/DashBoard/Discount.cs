using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.DashBoard
{
    public class Discount : SharedBase
    {
        public int DiscountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? MaxNumberOfUsage { get; set; }
        public decimal Value { get; set; }
        public decimal? FirstValueUse { get; set; }
        public bool IsDeleted { get; set; }

        public virtual List<ShopItem> ShopItems { get; set; }

    }
}
