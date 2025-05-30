using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Application.Services.SettingsManagement.Queries;
using E_Commerce.Domain.Enum;
using Generic.Response;
using MediatR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Caching
{
    public class CachingPlatformSettings
    {
        private Dictionary<PlatformSettingsCode, object> settings { get; set; }
        private IMediator _mediator { get; set; }
        public CachingPlatformSettings(IMediator mediator)
        {
            _mediator = mediator;
            this.GetPlatformSettingsAsync();
        }

        private async Task GetPlatformSettingsAsync()
        {
            try
            {
                BaseResponse<List<SettingsModel>> responseSettingss = await _mediator.Send(new SettingsQuery.Query());
                if (responseSettingss.IsSuccess)
                {
                    this.settings = new Dictionary<PlatformSettingsCode, object>();
                    foreach (SettingsModel setting in responseSettingss.DataObject)
                    {
                        Enum.TryParse(setting.Code, out PlatformSettingsCode settingCode);
                        this.settings.Add(settingCode, setting.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_log.txt", true))
                {
                    outputFile.WriteLine($"{nameof(GetPlatformSettingsAsync)}: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                    outputFile.WriteLine("*****************");
                }
            }
        }

        private dynamic GetSettingByCode(PlatformSettingsCode platformSettingsCode)
        {
            if (settings == null)
            {
                GetPlatformSettingsAsync().Wait();
            }
            else
            {
                return settings[platformSettingsCode];
            }
            return settings[platformSettingsCode];
        }

        public dynamic this[PlatformSettingsCode platformSettingsCode]
        {
            get => GetSettingByCode(platformSettingsCode);
        }
    }
}
