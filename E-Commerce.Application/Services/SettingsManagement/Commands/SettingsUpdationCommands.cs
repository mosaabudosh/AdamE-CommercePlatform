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
                string DMSSettingsPath = _cachingPlatformSettings[PlatformSettingsCode.DMSSettingsPath];
                Domain.Domains.Settings.PlatformSetting platformSetting = new Domain.Domains.Settings.PlatformSetting();
                try
                {
                    platformSetting = new Domain.Domains.Settings.PlatformSetting()
                    {
                        PlatformSettingId = request.SettingsModel.PlatformSettingId,
                        Code = request.SettingsModel.Code,
                        DataTypeId = request.SettingsModel.DataTypeId,
                        IsReadOnly = request.SettingsModel.IsReadOnly,
                        Name = request.SettingsModel.Name,
                        NameAr = request.SettingsModel.NameAr,
                        IsVisible = request.SettingsModel.IsVisible,
                        Value = request.SettingsModel.Value,
                        LastUpdateUserId = _userAccessor.GetCurrentUserId(),
                        LastUpdatedDate = DateTime.UtcNow,
                    };
                    _unitOfWork.GetRepository<Domain.Domains.Settings.PlatformSetting>().Update(platformSetting);
                    await _unitOfWork.CommitChanges();

                    if (platformSetting.DataTypeId == (int)Enums.PlatformSettingsDataTypes.File)
                    {
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{platformSetting.Name}",
                            FolderPath = $"{DMSSettingsPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        if (request.SettingsModel.FileCreationModel is not null)
                        {
                            folderAndFileCreationModel.FileCreationModels.Add(request.SettingsModel.FileCreationModel);
                        }
                        BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                        if (!createFolderAndOneFileResponse.IsSuccess)
                        {
                            return createFolderAndOneFileResponse;
                        }
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (platformSetting.DataTypeId == (int)Enums.PlatformSettingsDataTypes.File && request.SettingsModel.FileCreationModel is not null)
                    {
                        string folderPath = $"{DMSSettingsPath}\\{platformSetting.Name}_{platformSetting.PlatformSettingId}";
                        BaseResponse responseGetFileCreationModels = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                        if (!responseGetFileCreationModels.IsSuccess)
                        {
                            return responseGetFileCreationModels;
                        }
                    }
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
