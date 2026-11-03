using ArabazonPlatform.Domain.Domains.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Domain.Domains.Authorisation
{
    public class UserSetting : SharedBase
    {
        public int UserSettingId { get; set; }
        public int PlatformSettingId { get; set; }
        public Guid UserId { get; set; }
        public string? Value { get; set; }

        public virtual PlatformSetting PlatformSetting { get; set; }
        public virtual User User { get; set; }
    }
}
