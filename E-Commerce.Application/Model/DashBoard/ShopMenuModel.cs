using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ShopMenuModel
    {
        public int? ShopMenuId { get; set; }
        public int ShopId { get; set; }
        public string Name { get; set; }
        public string? NameAr { get; set; }
        public string Code { get; set; }
        public string? Shop { get; set; }
    }
}
