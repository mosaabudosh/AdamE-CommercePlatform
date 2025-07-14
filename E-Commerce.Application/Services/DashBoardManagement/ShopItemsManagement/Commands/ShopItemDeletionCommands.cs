using Adam.Logging.Interfaces;
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
    public class ShopItemDeletionCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public Guid ShopItemId { get; set; }
            public Command(Guid shopItemId)
            {
                ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    BaseResponse validationBaseResponse = ValidateDeletion(request.ShopItemId);
                    if (!validationBaseResponse.IsSuccess)
                    {
                        return validationBaseResponse;
                    }
                    ShopItem shopItem = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.ShopItemId == request.ShopItemId).SingleOrDefault();
                    _unitOfWork.GetRepository<ShopItem>().Delete(shopItem);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "ShopItemDeletionCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            private BaseResponse ValidateDeletion(Guid shopItemId)
            {
                try
                {
                    bool haveRelatedData = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(i => i.ShopItemId == shopItemId && (i.CartDetails != null || i.CustomerFavorites.Any() || i.OrderDetails != null )).Any();
                    if (haveRelatedData)
                    {
                        return new BaseResponse(false, string.Empty, ErrorCode.RelatedData);
                    }
                    return new BaseResponse(true, string.Empty);

                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    _exceptionService.LogErrorAsync(ex, nameof(ValidateDeletion), _userAccessor.GetCurrentUserId(), shopItemId);
                    return new BaseResponse(false, ex.Message);
                }
            }
        }
    }
}
