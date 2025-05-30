using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Authorisation
{
    public class AssignUnAssignScreensForRoleModel
    {
        public List<AssignUnAssignScreensForRole> AssignScreensForRoleModels { get; set; }
        public List<AssignUnAssignScreensForRole> UnScreensForRoleModels { get; set; }

    }

    public class AssignUnAssignScreensForRole
    {
        public int RoleScreenId { get; set; }
        public int RoleId { get; set; }
        public int ScreenId { get; set; }
        public string Name { get; set; }
        public List<RoleScreenActionModel> PageActions { get; set; }
    }

}
