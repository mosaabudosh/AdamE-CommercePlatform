using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Application.Services.SettingsManagement.Queries;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Caching
{
    public class CachingPlatformSettings
    {
        private Dictionary<PlatformSettingsCode, object> settings { get; set; }
        private PlatformInfoModel platformInfoModel { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }
        public bool isLodingData = false;
        public CachingPlatformSettings(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            this.GetPlatformSettingsAsync();
        }

        private async Task GetPlatformSettingsAsync()
        {
            try
            {
                List<SettingsModel> platformSettings = _unitOfWork.GetRepository<Domain.Domains.Settings.PlatformSetting>().GetByExpressionAsNoTracking().Select(s => new SettingsModel()
                {
                    PlatformSettingId = s.PlatformSettingId,
                    Code = s.Code,
                    DataTypeId = s.DataTypeId,
                    IsReadOnly = s.IsReadOnly,
                    Name = s.Name,
                    IsVisible = s.IsVisible,
                    Value = s.Value
                }).ToList();

                if (platformSettings is not null )
                {
                    this.settings = new Dictionary<PlatformSettingsCode, object>();
                    foreach (SettingsModel setting in platformSettings)
                    {
                        Enum.TryParse(setting.Code, out PlatformSettingsCode settingCode);
                        this.settings.Add(settingCode, setting.Value);
                    }
                    this.PlatformInfoFilling();
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_CachingPlatformSettings.txt", true))
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

        private async Task PlatformInfoFilling()
        {
            try
            {
                platformInfoModel = new PlatformInfoModel()
                {
                    PlatformName = settings[PlatformSettingsCode.Name]?.ToString(),
                    PlatformNameAr = settings[PlatformSettingsCode.NameAr]?.ToString(),
                    Logo = settings[PlatformSettingsCode.Logo]?.ToString(),
                    LogoAr = settings[PlatformSettingsCode.LogoAr]?.ToString(),
                    WaterMark = settings[PlatformSettingsCode.WaterMark]?.ToString(),
                    Address = settings[PlatformSettingsCode.Address]?.ToString(),
                    AddressAr = settings[PlatformSettingsCode.AddressAr]?.ToString(),
                    CRNumber = settings[PlatformSettingsCode.CRNumber]?.ToString(),
                    Email = settings[PlatformSettingsCode.Email]?.ToString(),
                    LicenceNumber = settings[PlatformSettingsCode.LicenceNumber]?.ToString(),
                    MobileNumber = settings[PlatformSettingsCode.MobileNumber]?.ToString(),
                    TaxNumber = settings[PlatformSettingsCode.TaxNumber]?.ToString(),
                    Website = settings[PlatformSettingsCode.Website]?.ToString()
                };
                isLodingData = true;
            }
            catch (Exception ex)
            {
                isLodingData = false;
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\__CachingPlatformSettings.txt", true))
                {
                    outputFile.WriteLine($"{nameof(PlatformInfoFilling)}: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                    outputFile.WriteLine("*****************");
                }
            }
        }

        private dynamic GetPlatformSettingByCode(PlatformSettingsCode platformSettingsCode)
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
            get => GetPlatformSettingByCode(platformSettingsCode);
        }

        public PlatformInfoModel GetPlatformInfo()
        {
            return platformInfoModel;
        }

        public void RefreshCachingData()
        {
            isLodingData = false;
            this.GetPlatformSettingsAsync();
        }

    }
}
