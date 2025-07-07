using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains.Authorisation;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.Domains.Notification;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.DashBoardManagement.CartsManagement.Commands
{
    public class CartUpdationCommands
    {
        public class Command : IRequest<BaseResponse<CartModel>>
        {
            public CartModel CartModel { get; set; }
            public Command(CartModel cartModel)
            {
                CartModel = cartModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse<CartModel>>
        {
            public async Task<BaseResponse<CartModel>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Cart exisitCart = _unitOfWork.GetRepository<Cart>().GetByExpression(p => p.CartId == request.CartModel.CartId).SingleOrDefault();

                    exisitCart.CartStatusId = request.CartModel.CartStatusId;
                    exisitCart.LastUpdatedDate = DateTime.UtcNow;
                    exisitCart.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    List<CartDetail> removeCartDetails = new List<CartDetail>();
                    List<CartDetail> removeSavedCartDetails = new List<CartDetail>();
                    List<CartDetail> newCartDetails = new List<CartDetail>();
                    List<CartDetail> modificationCartDetails = new List<CartDetail>();

                    if (!request.CartModel.CartDetailModels.Any())
                    {
                        removeCartDetails = _unitOfWork.GetRepository<CartDetail>().GetByExpressionAsNoTracking(p => p.CartId == request.CartModel.CartId).ToList();
                    }

                    List<Guid> stableCartDetailIds = request.CartModel.CartDetailModels.Where(p => p.CartDetailId.HasValue).Select(x => x.CartDetailId.Value).ToList();
                    
                    if (stableCartDetailIds.Any())
                    {
                        removeSavedCartDetails = _unitOfWork.GetRepository<CartDetail>().GetByExpressionAsNoTracking(p => !stableCartDetailIds.Contains(p.CartDetailId) && p.CartId == request.CartModel.CartId).ToList();

                        modificationCartDetails = _unitOfWork.GetRepository<CartDetail>().GetByExpressionAsNoTracking(p => stableCartDetailIds.Contains(p.CartDetailId) && p.CartId == request.CartModel.CartId).ToList();
                    }

                    if (modificationCartDetails.Any())
                    {
                        modificationCartDetails.Select(s =>
                        {
                            s.Quantity = request.CartModel.CartDetailModels.FirstOrDefault(p => p.CartDetailId == s.CartDetailId).Quantity;
                            return s;
                        }).ToList();

                        _unitOfWork.GetRepository<CartDetail>().UpdateRange(modificationCartDetails);
                    }

                    if (removeSavedCartDetails.Any())
                    {
                        removeCartDetails.AddRange(removeSavedCartDetails);
                    }

                    if (removeCartDetails.Any())
                    {
                        _unitOfWork.GetRepository<CartDetail>().RemoveRange(removeCartDetails);
                    }

                    newCartDetails = request.CartModel.CartDetailModels.Where(p => !p.CartDetailId.HasValue).Select(s => new CartDetail()
                    {
                        CartId = exisitCart.CartId,
                        Discount = s.Discount,
                        Price = s.Price,
                        Quantity = s.Quantity,
                        ShopItemId = s.ShopItemId,
                        Tax = s.Tax,
                        IsActive = true,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow
                    }).ToList();

                    if (newCartDetails.Any())
                    {
                        _unitOfWork.GetRepository<CartDetail>().AddRange(newCartDetails);
                    }
                 
                    await _unitOfWork.CommitChanges();

                    exisitCart.CartDetails =_unitOfWork.GetRepository<CartDetail>().GetByExpressionAsNoTracking(p => p.CartId == request.CartModel.CartId).ToList();

                    CartModel cartModel = new CartModel()
                    {
                        CartId = exisitCart.CartId,
                        CartStatusId = exisitCart.CartStatusId,
                        CustomerId = exisitCart.CustomerId,
                        CartDetailModels = exisitCart.CartDetails.Select(s => new CartDetailModel()
                        {
                            CartDetailId = s.CartDetailId,
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Tax = s.Tax,
                            Quantity = s.Quantity,
                        }).ToList()
                    };

                    return new BaseResponse<CartModel>(true, string.Empty, cartModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartUpdationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CartModel>(false, ex.Message);
                }
            }
        }
    }
}
