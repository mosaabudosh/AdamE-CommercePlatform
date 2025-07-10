using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ColorGroupModel
    {
        public int? ColorGroupId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<ColorModel>? ColorModels { get; set; }

    }
}
