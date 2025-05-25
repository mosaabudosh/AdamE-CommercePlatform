using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class RoleScreenActionModel
    {
        public int RoleScreenActionId { get; set; }
        public int RoleScreenId { get; set; }
        public int ActionId { get; set; }
        public int ScreenId { get; set; }
        public string ActionName { get; set; }
        public bool IsChecked { get; set; }
    }
}
