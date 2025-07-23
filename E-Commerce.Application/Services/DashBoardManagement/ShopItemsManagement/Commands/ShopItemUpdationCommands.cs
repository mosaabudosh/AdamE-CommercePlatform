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
    public class ShopItemUpdationCommands
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
                try
                {
                    ShopItem exisitShopItem = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.ShopItemId == request.ShopItemModel.ShopItemId.Value).SingleOrDefault();

                    exisitShopItem.TaxId = request.ShopItemModel.TaxId;
                    exisitShopItem.Cost = request.ShopItemModel.Cost;
                    exisitShopItem.ItemId = request.ShopItemModel.ItemId;
                    exisitShopItem.InStocks = request.ShopItemModel.InStocks;
                    exisitShopItem.ShopId = request.ShopItemModel.ShopId;
                    exisitShopItem.OutOfStocks = request.ShopItemModel.OutOfStocks;
                    exisitShopItem.Price = request.ShopItemModel.Price;
                    exisitShopItem.ProfitRate = request.ShopItemModel.ProfitRate;
                    exisitShopItem.ShopItemStatusId = request.ShopItemModel.ShopItemStatusId;
                    exisitShopItem.UnitGroupId = request.ShopItemModel.UnitGroupId;
                    exisitShopItem.ColorGroupId = request.ShopItemModel.ColorGroupId;
                    exisitShopItem.DefaultColorId = request.ShopItemModel.DefaultColorId;
                    exisitShopItem.DefaultUnitId = request.ShopItemModel.DefaultUnitId;
                    exisitShopItem.DiscountId = request.ShopItemModel.DiscountId;
                    exisitShopItem.IsFeatured = request.ShopItemModel.IsFeatured;

                    exisitShopItem.LastUpdatedDate = DateTime.UtcNow;
                    exisitShopItem.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    BaseResponse validationBaseResponse = ValidateUpdation(exisitShopItem);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }

                    string DMSShopItemPath = _cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath];

                    FolderAndFileCreationModel folderAndFileCreationModel = new FolderAndFileCreationModel()
                    {
                        FolderName = $"{exisitShopItem.ShopItemId}",
                        FolderPath = $"{DMSShopItemPath}",
                        FileCreationModels = request.ShopItemModel.FileCreationModels,
                    };

                    BaseResponse createFolderAndOneFileResponse = await _mediator.Send(new CreateFolderAndOneFileCommands.Command(folderAndFileCreationModel));
                    if (!createFolderAndOneFileResponse.IsSuccess)
                    {
                        return createFolderAndOneFileResponse;
                    }
                    if (request.ShopItemModel.FileCreationModels is not null && request.ShopItemModel.FileCreationModels.Any())
                    {
                        exisitShopItem.Image = exisitShopItem.ShopItemId.ToString();
                    }
                    _unitOfWork.GetRepository<ShopItem>().Update(exisitShopItem);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateUpdation(ShopItem shopItem)
            {
                try
                {
                    ShopItem existingRecord = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(x => (x.ItemId == shopItem.ItemId && x.ShopId == shopItem.ShopId) && x.ShopItemId != shopItem.ShopItemId).FirstOrDefault();

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
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateUpdation), _userAccessor.GetCurrentUserId(), shopItem);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
