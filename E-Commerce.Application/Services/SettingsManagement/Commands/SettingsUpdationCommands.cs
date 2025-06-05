using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.SettingsManagement.Commands
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
