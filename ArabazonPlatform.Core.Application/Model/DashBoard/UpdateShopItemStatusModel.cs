using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Model.DashBoard
{
    public class UpdateShopItemStatusModel
    {
        public Guid? ShopItemId { get; set; }
        public int ShopItemStatusId { get; set; }

    }
}
