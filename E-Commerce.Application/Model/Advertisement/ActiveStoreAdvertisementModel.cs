using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Advertisement
{
    public class ActiveStoreAdvertisementModel
    {
        public int AdvertisementId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string? TargetUrl { get; set; }
        public string? FileType { get; set; }
    }
}
