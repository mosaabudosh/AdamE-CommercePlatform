using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Domain.Domains.Authorisation
{
    public class ScreenAction : SharedBase
    {
        public int ScreenActionId { get; set; }
        public int ScreenId { get; set; }
        public int ActionId { get; set; }

        public virtual Screen Screen { get; set; }
        public virtual Action Action { get; set; }

    }
}
