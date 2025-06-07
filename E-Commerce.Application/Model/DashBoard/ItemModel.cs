using Adam.Generic.Generic;
using E_Commerce.Domain.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class ItemModel
    {
        public Guid? ItemId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string Code { get; set; }
        public string? Image { get; set; }
        public string Descreption { get; set; }
        public bool? IsBlock { get; set; }
        public int CategoryId { get; set; }
        public string? Category { get; set; }
        public List<FileCreationModel>? FileCreationModels { get; set; }
        public List<string>? Images { get; set; }


    }
}
