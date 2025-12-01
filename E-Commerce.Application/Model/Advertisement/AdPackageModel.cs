using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Advertisement
{
    public class AdPackageModel
    {
        public int AdPackageId { get; set; }
        public string Name { get; set; }
        public string? NameAr { get; set; }
        public string Description { get; set; }
        public string? DescriptionAr { get; set; }
        public decimal Price { get; set; }
        public int DurationDays { get; set; }
        public int AdTypeId { get; set; }
        public string? AdType { get; set; }
    }
}
