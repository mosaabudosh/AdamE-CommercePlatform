using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.SettingsManagement.Commands
{
    public class SettingsUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public SettingsModel SettingsModel { get; set; }
            public Command(SettingsModel settingsModel)
            {
                this.SettingsModel = settingsModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Domain.Domains.Settings.PlatformSetting exisitPlatformSetting = await _unitOfWork.GetRepository<Domain.Domains.Settings.PlatformSetting>().GetById(request.SettingsModel.PlatformSettingId);

                    exisitPlatformSetting.Code = request.SettingsModel.Code;
                    exisitPlatformSetting.DataTypeId = request.SettingsModel.DataTypeId;
                    exisitPlatformSetting.IsReadOnly = request.SettingsModel.IsReadOnly;
                    exisitPlatformSetting.Name = request.SettingsModel.Name;
                    exisitPlatformSetting.NameAr = request.SettingsModel.NameAr;
                    exisitPlatformSetting.IsVisible = request.SettingsModel.IsVisible;
                    exisitPlatformSetting.Value = request.SettingsModel.Value;
                    exisitPlatformSetting.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitPlatformSetting.LastUpdatedDate = DateTime.UtcNow;

                    await _unitOfWork.CommitChanges();
                    _cachingPlatformSettings.RefreshCachingData();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "SettingsUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

        }
    }
}
