using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.CMS
{
    public class StaticPage : SharedBase
    {
        public int StaticPageId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }
        public string? Content { get; set; }
        public string? ContentAr { get; set; }
    }
}
