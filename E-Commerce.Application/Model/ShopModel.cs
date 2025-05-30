using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model
{
    public class ShopModel
    {
        public int? ShopId { get; set; }
        public string Name { get; set; }
        public string CommercialName { get; set; }
        public bool HasBranch { get; set; }
        public bool? IsBlock { get; set; }
        public string? Logo { get; set; }
        public string Adresse { get; set; }
        public int? NoOfBranch { get; set; }
        public int? NoOfVendor { get; set; }
        public string PhoneNumber { get; set; }
        public string PersonConcateName { get; set; }
        public string? Descreption { get; set; }
        public string? About { get; set; }
        public string Code { get; set; }
        public int? CityId { get; set; }
        public int? ParentShopId { get; set; }
        public string City { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }

    }
}
