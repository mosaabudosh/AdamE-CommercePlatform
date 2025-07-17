using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.DashBoard
{
    public class UpdateOrderStatusModel
    {
        public Guid? OrderId { get; set; }
        public int NewOrderStatusId { get; set; }
    }
}
