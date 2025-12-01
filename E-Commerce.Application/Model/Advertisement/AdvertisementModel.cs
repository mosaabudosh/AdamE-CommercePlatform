using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Advertisement
{
    public class AdvertisementModel
    {
        public int AdvertisementId { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public string? TargetUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AdvertisementStatusId { get; set; }
        public int? ShopAdPackageId { get; set; }
        public int? AdPackageId { get; set; }
        public string? AdPackageName { get; set; }
        public string? AdvertisementStatus { get; set; }
        public string? ShopAdPackageName { get; set; }
        public string? AdImpressions { get; set; }
        public string? AdClicks { get; set; }

        public FileCreationModel? FileCreationModel { get; set; }

    }
}
