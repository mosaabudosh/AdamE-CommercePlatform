using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Image { get; set; }
        public string Descreption { get; set; }
        public bool? IsBlock { get; set; }
        public int? ParentCategoryId { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
    }
}
