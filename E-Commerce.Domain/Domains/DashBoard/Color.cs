using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class Color : SharedBase
    {
        public int ColorId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Value { get; set; }
        public int ColorGroupId { get; set; }

        public virtual ColorGroup ColorGroup { get; set; }
    }
}
