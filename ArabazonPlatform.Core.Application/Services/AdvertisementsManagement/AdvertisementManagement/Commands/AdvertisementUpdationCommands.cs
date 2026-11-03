using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AdvertisementsManagement.AdvertisementManagement.Commands
{
    public class AdvertisementUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public AdvertisementModel AdvertisementModel { get; set; }
            public Command(AdvertisementModel advertisementModel)
            {
                AdvertisementModel = advertisementModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSAdPath = _cachingPlatformSettings[PlatformSettingsCode.DMSAdPath];

                try
                {
                    Advertisement exisitAdvertisement = await _unitOfWork.GetRepository<Advertisement>().GetById(request.AdvertisementModel.AdvertisementId);

                    exisitAdvertisement.Title = request.AdvertisementModel.Title.Trim();
                    exisitAdvertisement.ImageUrl = !string.IsNullOrEmpty(request.AdvertisementModel.ImageUrl) ? request.AdvertisementModel.ImageUrl.Trim() : null;
                    exisitAdvertisement.TargetUrl = !string.IsNullOrEmpty(request.AdvertisementModel.TargetUrl) ? request.AdvertisementModel.TargetUrl.Trim() : null;
                    exisitAdvertisement.AdPackageId = request.AdvertisementModel.AdPackageId;
                    exisitAdvertisement.StartDate = request.AdvertisementModel.StartDate;
                    exisitAdvertisement.EndDate = request.AdvertisementModel.EndDate;
                    exisitAdvertisement.AdvertisementStatusId = request.AdvertisementModel.AdvertisementStatusId;
                    exisitAdvertisement.ShopAdPackageId = request.AdvertisementModel.ShopAdPackageId;
                    exisitAdvertisement.LastUpdatedDate = DateTime.UtcNow;
                    exisitAdvertisement.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    await _unitOfWork.CommitChanges();

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitAdvertisement.AdvertisementId}",
                        FolderPath = $"{DMSAdPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };

                    if (request.AdvertisementModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.AdvertisementModel.FileCreationModel);
                    }

                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    if (request.AdvertisementModel.FileCreationModel is not null)
                    {
                        exisitAdvertisement.ImageUrl = exisitAdvertisement.AdvertisementId.ToString();
                    }
                    else
                    {
                        exisitAdvertisement.ImageUrl = null;
                    }

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    if (request.AdvertisementModel.FileCreationModel is not null && request.AdvertisementModel.AdvertisementId != 0)
                    {
                        string folderPath = $"{DMSAdPath}\\{request.AdvertisementModel.AdvertisementId}";
                        BaseResponse responseDeleteFolderByPathCommands = await _mediator.Send(new DeleteFolderByPathCommands.Command(folderPath));
                        if (!responseDeleteFolderByPathCommands.IsSuccess)
                        {
                            return responseDeleteFolderByPathCommands;
                        }
                    }

                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "AdvertisementUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            
        }
    }
}
