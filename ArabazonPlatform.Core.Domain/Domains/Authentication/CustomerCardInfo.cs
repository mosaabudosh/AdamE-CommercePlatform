using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Authentication
{
    public class CustomerCardInfo : SharedBase
    {
        public int CustomerCardInfoId { get; set; }
        public string CardNumber { get; set; }
        public string Expiry { get; set; }
        public bool IsRememberCard { get; set; }
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual List<Order> Orders { get; set; }

    }
}
