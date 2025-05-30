using Adam.Generic.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Model.Settings
{
    public class SettingsModel
    {
        public int PlatformSettingId { get; set; }
        public string? Name { get; set; }
        public string? NameAr { get; set; }
        public string? Value { get; set; }
        public string? Code { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsVisible { get; set; }
        public int DataTypeId { get; set; }
        public string DataType { get; set; }
        public FileCreationModel? FileCreationModel { get; set; }
    }
}
