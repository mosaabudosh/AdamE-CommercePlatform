using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.Domains.Settings;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.SettingsManagement.Queries
{
    public class SettingsQuery
    {
        public class Query : IRequest<BaseResponse<List<SettingsModel>>> 
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }

            public Query(int pageNumber, int pageSize)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Query, BaseResponse<List<SettingsModel>>>
        {
            public async Task<BaseResponse<List<SettingsModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<SettingsModel> platformSettings = _unitOfWork.GetRepository<Domain.Domains.Settings.PlatformSetting>().GetByExpressionAsNoTracking().Select(s => new SettingsModel()
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
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();

                    //if (platformSettings.Any(p => p.DataTypeId == (int)Enums.PlatformSettingsDataTypes.File && !string.IsNullOrEmpty(p.Value)))
                    //{
                    //    string DMSSettingsPath = _cachingPlatformSettings[PlatformSettingsCode.DMSSettingsPath];
                    //    List<string> filePaths = platformSettings.Where(p => p.DataTypeId == (int)Enums.PlatformSettingsDataTypes.File && !string.IsNullOrEmpty(p.Value)).Select(s => { return $"{DMSSettingsPath}\\{s.Name}"; }).ToList();
                    //    BaseResponse<List<FileCreationModel>> responseGetFileCreationModels = await _mediator.Send(new FilesByFilePathQuery.Query(filePaths));
                    //    if (!responseGetFileCreationModels.IsSuccess)
                    //    {
                    //        return new BaseResponse<List<SettingsModel>>(false, responseGetFileCreationModels.Message);
                    //    }
                    //    if (responseGetFileCreationModels.DataObject.Any())
                    //    {
                    //        foreach (SettingsModel platformSetting in platformSettings.Where(p => p.DataTypeId == (int)Enums.PlatformSettingsDataTypes.File && !string.IsNullOrEmpty(p.Value)))
                    //        {
                    //            platformSetting.FileCreationModel = responseGetFileCreationModels.DataObject.FirstOrDefault(p => p.FileName == platformSetting.Value);
                    //        }
                    //    }
                    //}

                    return new BaseResponse<List<SettingsModel>>(true, string.Empty, string.Empty, platformSettings);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "SettingsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<SettingsModel>>(false, ex.Message);
                }
            }
        }
    }
}
