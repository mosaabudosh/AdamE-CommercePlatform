using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Authentication
{
    public class CustomerDeliveryAddress : SharedBase
    {
        public int CustomerDeliveryAddressId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? AddressLabel { get; set; }
        public bool IsDefaultAddress { get; set; }
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }

    }
}
