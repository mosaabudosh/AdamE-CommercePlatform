using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class UnitGroupModel
    {
        public int UnitGroupId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public List<UnitModel>? UnitModels { get; set; }

    }
}
