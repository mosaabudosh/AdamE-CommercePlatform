using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class UserRoleModel
    {
        public int UserRoleId { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public string? Role { get; set; }
    }
}
