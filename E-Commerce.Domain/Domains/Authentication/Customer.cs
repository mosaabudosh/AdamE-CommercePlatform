using E_Commerce.Domain.Domains.Authentication;
using E_Commerce.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Authorisation
{
    public class Customer : BaseUser
    {
        public Guid CustomerId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? ProfileImage { get; set; }
        public string Adresse { get; set; }
        public bool IsBlock { get; set; }
        public int UserTypeId { get; set; }
        public int GenderId { get; set; }
        public int CityId { get; set; }
        public string? OTPCode { get; set; }

        public virtual List<Cart> Carts { get; set; }
        public virtual List<Order> Orders { get; set; }
        public virtual List<CustomerLogin> CustomerLogins { get; set; }
    }
}
