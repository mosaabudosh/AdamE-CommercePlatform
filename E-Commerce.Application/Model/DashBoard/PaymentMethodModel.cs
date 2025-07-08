using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class PaymentMethodModel
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Code { get; set; }
        public string? Icon { get; set; }
        public bool IsActive { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
    }
}
