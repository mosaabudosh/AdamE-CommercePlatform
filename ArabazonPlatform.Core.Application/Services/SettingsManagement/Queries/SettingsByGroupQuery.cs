using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.SettingsManagement.Queries
{
    public class SettingsByGroupQuery
    {
        public class Query : IRequest<BaseResponse<List<SettingsModel>>>
        {
            public string GroupName { get; set; }
            public Query(string groupName)
            {
                GroupName = groupName;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<SettingsModel>>>
        {
            public async Task<BaseResponse<List<SettingsModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<SettingsModel> platformSettings = _unitOfWork.GetRepository<Domain.Domains.Settings.PlatformSetting>().GetByExpressionAsNoTracking(p => p.Group == request.GroupName).Select(s => new SettingsModel()
                    {
                        PlatformSettingId = s.PlatformSettingId,
                        Code = s.Code,
                        DataType = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.DataType.Name : s.DataType.NameAr,
                        DataTypeId = s.DataTypeId,
                        IsReadOnly = s.IsReadOnly,
                        Name = _userAccessor.GetCurrentUserLanguage() == ArabazonPlatform.Generic.Enum.LanguageCode.enUS ? s.Name : s.NameAr,
                        IsVisible = s.IsVisible,
                        Value = s.Value,
                        NameAr = s.NameAr
                    }).ToList();

                    return new BaseResponse<List<SettingsModel>>(true, string.Empty, string.Empty, platformSettings);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "SettingsByGroupQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<SettingsModel>>(false, ex.Message);
                }
            }
        }
    }
}
