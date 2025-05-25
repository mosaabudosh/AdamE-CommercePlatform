using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class ScreenActionModel
    {
        public int ScreenActionId { get; set; }
        public int ScreenId { get; set; }
        public int ActionId { get; set; }
        public string? Action { get; set; }

    }
}
