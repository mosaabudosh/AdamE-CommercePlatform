using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Domain.Domains.DashBoard;

namespace E_Commerce.Domain.Domains.Authorisation
{
    public class BaseUser : SharedBase
    {
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? LastName { get; set; }
        public string? FirstNameAr { get; set; }
        public string? SecondNameAr { get; set; }
        public string? LastNameAr { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? MobileNumber { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }

        public virtual Gender Gender { get; set; }
        public virtual City City { get; set; }
    }
}
