using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
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

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
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
                        Image = request.ShopItemModel.Image,
                        InStocks = request.ShopItemModel.InStocks,
                        ItemId = request.ShopItemModel.ItemId,
                        OutOfStocks = request.ShopItemModel.OutOfStocks,
                        Price = request.ShopItemModel.Price,
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
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
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
