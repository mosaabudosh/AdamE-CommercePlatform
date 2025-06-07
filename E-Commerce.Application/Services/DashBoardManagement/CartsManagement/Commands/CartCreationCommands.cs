using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Domain.Domains;
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

namespace E_Commerce.Application.Services.DashBoardManagement.CartsManagement.Commands
{
    public class CartCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CartModel CartModel { get; set; }
            public Command(CartModel cartModel)
            {
                CartModel = cartModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
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
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }


        }
    }
}
