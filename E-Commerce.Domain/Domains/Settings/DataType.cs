using E_Commerce.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Domains.Settings
{
    public class DataType : SharedBase
    {
        public int DataTypeId { get; set; }
        public string Name { get; set; }
        public string NameAr { get; set; }

        public virtual List<PlatformSetting> PlatformSettings { get; set; }

    }
}
