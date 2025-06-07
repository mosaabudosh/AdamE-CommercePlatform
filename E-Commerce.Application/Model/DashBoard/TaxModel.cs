using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class TaxModel
    {
        public int? TaxId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public decimal Value { get; set; }
        public string Code { get; set; }
    }
}
