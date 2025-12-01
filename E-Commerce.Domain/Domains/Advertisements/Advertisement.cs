using E_Commerce.Domain.Domains.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Advertisements
{
    public class Advertisement : SharedBase
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

        public virtual AdvertisementStatus AdvertisementStatus { get; set; }
        public virtual ShopAdPackage ShopAdPackage { get; set; }
        public virtual AdPackage AdPackage { get; set; }
        public virtual ICollection<AdImpression> AdImpressions { get; set; }
        public virtual ICollection<AdClick> AdClicks { get; set; }

    }
}
