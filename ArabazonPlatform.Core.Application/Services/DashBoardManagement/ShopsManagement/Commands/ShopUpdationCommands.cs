using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Generic.Constants.Constants;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.ShopsManagement.Commands
{
    public class ShopUpdationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ShopModel ShopModel { get; set; }
            public Command(ShopModel shopModel)
            {
                ShopModel = shopModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Shop exisitShop = await _unitOfWork.GetRepository<Shop>().GetById(request.ShopModel.ShopId.Value);

                    exisitShop.Name = request.ShopModel.Name.Trim();
                    exisitShop.Code = request.ShopModel.Code.Trim();
                    exisitShop.Descreption = request.ShopModel.Descreption?.Trim();
                    exisitShop.Logo = request.ShopModel.FileCreationModel is not null ? request.ShopModel.ShopId.ToString() : null;
                    exisitShop.PhoneNumber = request.ShopModel.PhoneNumber.Trim();
                    exisitShop.PersonConcateName = request.ShopModel.PersonConcateName.Trim();
                    exisitShop.ParentShopId = request.ShopModel.ParentShopId;
                    exisitShop.NoOfVendor = request.ShopModel.NoOfVendor;
                    exisitShop.NoOfBranch = request.ShopModel.NoOfBranch;
                    exisitShop.HasBranch = request.ShopModel.HasBranch;
                    exisitShop.About = request.ShopModel.About?.Trim();
                    exisitShop.Adresse = request.ShopModel.Adresse.Trim();
                    exisitShop.CityId = request.ShopModel.CityId;
                    exisitShop.CommercialName = request.ShopModel.CommercialName.Trim();
                    exisitShop.IsBlock = request.ShopModel.IsBlock.HasValue ? request.ShopModel.IsBlock.Value : false;
                    exisitShop.IsViewInHome = request.ShopModel.IsViewInHome;
                    exisitShop.Latitude = request.ShopModel.Latitude;
                    exisitShop.Longitude = request.ShopModel.Longitude;
                    exisitShop.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                    exisitShop.LastUpdatedDate = DateTime.UtcNow;

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitShop);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    string DMSShopPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopPath];
                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitShop.ShopId}",
                        FolderPath = $"{DMSShopPath}",
                        FileCreationModels = new List<FileCreationModel>(),
                    };
                    if (request.ShopModel.FileCreationModel is not null)
                    {
                        folderAndFileCreationModel.FileCreationModels.Add(request.ShopModel.FileCreationModel);
                    }
                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }

                    await _unitOfWork.CommitChanges();

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(Shop shop)
            {
                try
                {
                    Shop existingRecord = _unitOfWork.GetRepository<Shop>().GetByExpressionAsNoTracking(x => x.Name == shop.Name && x.CommercialName == shop.CommercialName && x.ShopId != shop.ShopId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), shop);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
