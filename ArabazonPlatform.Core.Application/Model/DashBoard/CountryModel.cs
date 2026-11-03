using ArabazonPlatform.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class CountryModel
    {
        public int CountryId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Banner { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
    }
}
