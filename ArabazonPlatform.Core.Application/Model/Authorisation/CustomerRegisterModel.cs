using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.Authorisation
{
    public class CustomerRegisterModel
    {
        public Guid CustomerId { get; set; }
        public string? Password { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? OTPCode { get; set; }
    }
}
