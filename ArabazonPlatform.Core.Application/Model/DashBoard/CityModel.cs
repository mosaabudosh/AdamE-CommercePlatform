using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class CityModel
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public int CountryId { get; set; }
        public string? Country { get; set; }
    }
}
