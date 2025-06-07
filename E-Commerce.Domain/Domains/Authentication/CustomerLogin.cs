using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Authentication
{
    public class CustomerLogin : SharedBase
    {
        public Guid CustomerLoginId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public long RefreshTokenExpirationDate { get; set; }
        public DateTime LoginDate { get; set; }
        public DateTime? LogOutDate { get; set; }
        public Guid CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
