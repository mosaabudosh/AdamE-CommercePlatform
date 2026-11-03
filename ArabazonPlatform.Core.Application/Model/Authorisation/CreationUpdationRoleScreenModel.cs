using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.Authorisation
{
    public class CreationUpdationRoleScreenModel
    {
        public int RoleId { get; set; }
        public List<AssignUnAssignScreensForRole> AssignScreensForRole { get; set; }

    }
}
