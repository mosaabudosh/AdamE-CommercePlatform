using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CartsManagement.Commands
{
    public class CartCreationCommands
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
                    Cart cart = new Cart()
                    {
                        CartStatusId = request.CartModel.CartStatusId,
                        CustomerId = request.CartModel.CustomerId,
                        CreationDate = DateTime.UtcNow,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        IsActive = true,
                        CartDetails = request.CartModel.CartDetailModels.Select(s => new CartDetail()
                        {
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Quantity = s.Quantity,
                            Tax = s.Tax,
                            CreationDate = DateTime.UtcNow,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            IsActive = true,
                        }).ToList()
                    };

                    _unitOfWork.GetRepository<Cart>().Insert(cart);
                    await _unitOfWork.CommitChanges();

                    CartModel cartModel = new CartModel()
                    {
                        CartId = cart.CartId,
                        CartStatusId = cart.CartStatusId,
                        CustomerId = cart.CustomerId,
                        CartDetailModels = cart.CartDetails.Select(s => new CartDetailModel()
                        {
                            CartDetailId = s.CartDetailId,
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Tax = s.Tax,
                            Quantity = s.Quantity,
                        }).ToList()
                    };
                    return new BaseResponse<CartModel>(true,string.Empty, cartModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CartModel>(false, ex.Message);
                }
            }


        }
    }
}
