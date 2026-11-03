using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.Authorisation
{
    public class CustomerDeliveryAddressModel
    {
        public int? CustomerDeliveryAddressId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string? AddressLabel { get; set; }
        public bool IsDefaultAddress { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerMobileNumber { get; set; }

    }
}
