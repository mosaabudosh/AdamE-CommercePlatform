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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Commands
{
    public class ShopItemCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public ShopItemModel ShopItemModel { get; set; }
            public Command(ShopItemModel shopItemModel)
            {
                ShopItemModel = shopItemModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator, CachingPlatformSettings _cachingPlatformSettings) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                string DMSShopItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath];
                try
                {
                    ShopItem shopItem = new ShopItem()
                    {
                        ShopId = request.ShopItemModel.ShopId,
                        TaxId = request.ShopItemModel.TaxId,
                        ShopItemStatusId = request.ShopItemModel.ShopItemStatusId,
                        UnitGroupId = request.ShopItemModel.UnitGroupId,
                        Cost = request.ShopItemModel.Cost,
                        ProfitRate = request.ShopItemModel.ProfitRate,
                        Image = request.ShopItemModel.ShopItemId.ToString(),
                        InStocks = request.ShopItemModel.InStocks,
                        ItemId = request.ShopItemModel.ItemId,
                        OutOfStocks = request.ShopItemModel.OutOfStocks,
                        Price = request.ShopItemModel.Price,
                        IsFeatured = request.ShopItemModel.IsFeatured,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true
                    };
                    BaseResponse validationBaseResponse = ValidateCreation(shopItem);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    _unitOfWork.GetRepository<ShopItem>().Insert(shopItem);
                    await _unitOfWork.CommitChanges();

                    if (request.ShopItemModel.FileCreationModels is not null && request.ShopItemModel.FileCreationModels.Any())
                    {
                        request.ShopItemModel.ShopItemId = shopItem.ShopItemId;
                        shopItem.Image = shopItem.ShopItemId.ToString();
                        await _unitOfWork.CommitChanges();
                        FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                        {
                            FolderName = $"{shopItem.ShopItemId}",
                            FolderPath = $"{DMSShopItemPath}",
                            FileCreationModels = request.ShopItemModel.FileCreationModels
                        };
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
                    if (request.ShopItemModel.FileCreationModels is not null && request.ShopItemModel.FileCreationModels.Any() && request.ShopItemModel.ShopItemId.HasValue)
                    {
                        string folderPath = $"{DMSShopItemPath}\\{request.ShopItemModel.ShopItemId}";
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
                    await _exceptionService.LogErrorAsync(ex, "ShopItemCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateCreation(ShopItem shopItem)
            {
                try
                {
                    ShopItem existingRecord = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(x => x.ItemId == shopItem.ItemId && x.ShopId == shopItem.ShopId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateCreation), _userAccessor.GetCurrentUserId(), shopItem);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
