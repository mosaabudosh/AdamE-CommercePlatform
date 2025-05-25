using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class ScreenModel
    {
        public int ScreenId { get; set; }
        public string Name { get; set; }
        public string InMenue { get; set; }
        public string ModuleName { get; set; }
        public string Status { get; set; }
        public List<ScreenActionModel> ScreenActions { get; set; }
    }
}
