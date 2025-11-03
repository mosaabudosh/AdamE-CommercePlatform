using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ShopCategoryModel
    {
        public int? ShopCategoryId { get; set; }
        public int ShopId { get; set; }
        public int CategoryId { get; set; }
        public string? Shop { get; set; }
        public string? Category { get; set; }
    }
}
