using ArabazonPlatform.Domain.Domains.Authorisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Settings
{
    public class PlatformSetting : SharedBase
    {
        public int PlatformSettingId { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Value { get; set; }
        public string? Code { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVisible { get; set; }
        public int DataTypeId { get; set; }
        public string? Group { get; set; }

        public virtual DataType DataType { get; set; }
        public virtual List<UserSetting> UserSettings { get; set; }


    }
}
