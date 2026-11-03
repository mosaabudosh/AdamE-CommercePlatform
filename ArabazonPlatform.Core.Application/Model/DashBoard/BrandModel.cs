using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class BrandModel
    {
        public int? BrandId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Image { get; set; }
        public string? Descreption { get; set; }
        public bool IsBlock { get; set; }
        public bool IsViewInHome { get; set; }
        public bool IsViewInFooter { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
    }
}
