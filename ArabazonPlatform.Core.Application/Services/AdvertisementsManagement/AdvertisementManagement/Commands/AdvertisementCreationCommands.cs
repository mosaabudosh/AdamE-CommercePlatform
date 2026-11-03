using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.Advertisements;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Generic.Constants.Constants;
using static ArabazonPlatform.Domain.Enum.Enums;
using ArabazonPlatform.Generic.Response;
using Adam.Logging.Interfaces;

namespace ArabazonPlatform.Application.Services.AdvertisementsManagement.AdvertisementManagement.Commands
{
    public class AdvertisementCreationCommands
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
                    Advertisement advertisement = new Advertisement()
                    {
                        AdvertisementId = 0,
                        Title = request.AdvertisementModel.Title,
                        AdPackageId = request.AdvertisementModel.AdPackageId,
                        AdvertisementStatusId = request.AdvertisementModel.AdvertisementStatusId,
                        EndDate = request.AdvertisementModel.EndDate,
                        StartDate = request.AdvertisementModel.StartDate,
                        ImageUrl = request.AdvertisementModel.ImageUrl,
                        ShopAdPackageId = request.AdvertisementModel.ShopAdPackageId,
                        TargetUrl = request.AdvertisementModel.TargetUrl,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(advertisement);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Advertisement>().Insert(advertisement);
                    await _unitOfWork.CommitChanges();

                    if (request.AdvertisementModel.FileCreationModel is not null)
                    {
                        advertisement.ImageUrl = advertisement.AdvertisementId.ToString();
                        await _unitOfWork.CommitChanges();
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{advertisement.AdvertisementId}",
                            FolderPath = $"{DMSAdPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.AdvertisementModel.FileCreationModel);
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
                    await _exceptionService.LogErrorAsync(ex, "AdvertisementCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Advertisement advertisement)
            {
                try
                {
                    Advertisement existingRecord = _unitOfWork.GetRepository<Advertisement>().GetByExpressionAsNoTracking(x => x.Title == advertisement.Title && x.AdvertisementStatusId == advertisement.AdvertisementStatusId).FirstOrDefault();

                    if (existingRecord is not null)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.AlreadyExsist);
                    }
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), advertisement);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
