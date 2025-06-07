using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.DashBoard
{
    public class PaymentMethod : SharedBase
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Code { get; set; }
        public string? Icon { get; set; }

        public virtual List<Order> Orders { get; set; }

    }
}
