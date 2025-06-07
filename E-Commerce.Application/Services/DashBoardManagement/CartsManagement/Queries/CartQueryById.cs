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

namespace E_Commerce.Application.Services.DashBoardManagement.CartsManagement.Queries
{
    public class CartQueryById
    {
        public class Query : IRequest<BaseResponse<CartModel>>
        {
            public Guid CartId { get; set; }
            public Query(Guid cartId)
            {
                CartId = cartId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<CartModel>>
        {
            public async Task<BaseResponse<CartModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CartModel cartModel = _unitOfWork.GetRepository<Cart>().GetByExpressionAsNoTracking(p => p.CartId == request.CartId).Select(s => new CartModel()
                    {
                        CartId = s.CartId,
                        CartStatusId = s.CartStatusId,
                        CustomerId = s.CustomerId,
                        CartStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.CartStatus.Name : s.CartStatus.NameAr,
                        Customer = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.Customer.FirstName} {s.Customer.LastName}" : $"{s.Customer.FirstNameAr} {s.Customer.LastNameAr}",
                        CartDetailModels = s.CartDetails.Select(p => new CartDetailModel()
                        {
                            CartDetailId = p.CartDetailId,
                            Discount = p.Discount,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            ShopItemId = p.ShopItemId,
                            Tax = p.Tax,
                            ItemName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.ShopItem.Item.Name : p.ShopItem.Item.NameAr
                        }).ToList()
                    }).FirstOrDefault();
                    return new BaseResponse<CartModel>(true, string.Empty, string.Empty, cartModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CartModel>(false, ex.Message);
                }
            }
        }
    }
}
