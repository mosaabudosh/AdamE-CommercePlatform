using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Commands;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopsManagement.Commands
{
    public class ShopCreationCommands
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
                    Shop shop = new Shop()
                    {
                        Name = request.ShopModel.Name.Trim(),
                        Code = request.ShopModel.Code.Trim(),
                        Descreption = request.ShopModel.Descreption?.Trim(),
                        PhoneNumber = request.ShopModel.PhoneNumber.Trim(),
                        PersonConcateName = request.ShopModel.PersonConcateName.Trim(),
                        ParentShopId = request.ShopModel.ParentShopId,
                        NoOfVendor = request.ShopModel.NoOfVendor,
                        NoOfBranch = request.ShopModel.NoOfBranch,
                        HasBranch = request.ShopModel.HasBranch,
                        About = request.ShopModel.About?.Trim(),
                        Adresse = request.ShopModel.Adresse.Trim(),
                        CityId = request.ShopModel.CityId,
                        CommercialName = request.ShopModel.CommercialName.Trim(),
                        IsBlock = false,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(shop);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<Shop>().Insert(shop);
                    await _unitOfWork.CommitChanges();

                    if (request.ShopModel.FileCreationModel is not null)
                    {
                        string DMSShopPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopPath];
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{shop.ShopId.ToString()}",
                            FolderPath = $"{DMSShopPath}",
                            FileCreationModels = new List<FileCreationModel>(),
                        };
                        folderAndFileCreationModel.FileCreationModels.Add(request.ShopModel.FileCreationModel);
                        BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                        if (!createFolderAndOneFileResponse.IsSuccess)
                        {
                            return createFolderAndOneFileResponse;
                        }
                        await _unitOfWork.CommitChanges();
                    }

                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(Shop shop)
            {
                try
                {
                    Shop existingRecord = _unitOfWork.GetRepository<Shop>().GetByExpressionAsNoTracking(x => x.Name == shop.Name || x.CommercialName == shop.CommercialName || x.Code == shop.Code).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), shop);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
