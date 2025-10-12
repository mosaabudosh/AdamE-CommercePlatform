using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Store
{
    public class DashboardFiltersModel
    {
        public List<DDLDataSource> Brands { get; set; }
        public List<DDLDataSource> Categories { get; set; }
        public List<DDLDataSource> Sizes { get; set; }
        public List<DDLDataSource> Colors { get; set; }
        public List<DDLDataSource> Discounts { get; set; }
        public List<DDLDataSource> Shops { get; set; }
    }
}
