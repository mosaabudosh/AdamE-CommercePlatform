using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class DashboardFooterModel
    {
        public List<DDLDataSource> TopBrands { get; set; }
        public List<DDLDataSource> WomenFashion { get; set; }
        public List<DDLDataSource> MenFashion { get; set; }
        public List<DDLDataSource> Beauty { get; set; }
        public List<DDLDataSource> Kids { get; set; }
    }
}
