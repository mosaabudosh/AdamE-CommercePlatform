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
    public class CartsQuery
    {
        public class Query : IRequest<BaseResponse<List<CartModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public Guid? CustomerId { get; set; }
            public int? CartStatusId { get; set; }
            public Guid? ShopItemId { get; set; }

            public Query(int pageNumber, int pageSize, Guid? customerId = null, int? cartStatusId = null, Guid? shopItemId = null)
            {
                PageNumber = pageNumber;
                PageSize = pageSize;
                CustomerId = customerId;
                CartStatusId = cartStatusId;
                ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CartModel>>>
        {
            public async Task<BaseResponse<List<CartModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CartModel> cart = _unitOfWork.GetRepository<Cart>().GetByExpressionAsNoTracking(p => (!request.CustomerId.HasValue || p.CustomerId == request.CustomerId) && (!request.CartStatusId.HasValue || p.CartStatusId == request.CartStatusId) && (!request.ShopItemId.HasValue || p.CartDetails.Any(x => x.ShopItemId == request.ShopItemId))).Select(s => new CartModel()
                    {
                        CartId = s.CartId,
                        CartStatusId = s.CartStatusId,
                        CustomerId = s.CustomerId,
                        CartStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.CartStatus.Name : s.CartStatus.NameAr,
                        Customer = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.Customer.FirstName} {s.Customer.SecondName} {s.Customer.LastName}" : $"{s.Customer.FirstNameAr} {s.Customer.SecondNameAr} {s.Customer.LastNameAr}",
                        CartDetailModels = s.CartDetails.Select(p => new CartDetailModel()
                        {
                            CartDetailId = p.CartDetailId,
                            Discount = p.Discount,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            ShopItemId = p.ShopItemId,
                            Tax = p.Tax,
                            ItemName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.ShopItem.Name : p.ShopItem.NameAr
                        }).ToList(),
                    }).Skip(request.PageNumber * request.PageSize).Take(request.PageSize).ToList();
                    return new BaseResponse<List<CartModel>>(true, string.Empty, string.Empty, cart);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CartsQuery", _userAccessor.GetCurrentUserId(), request);
                    return new BaseResponse<List<CartModel>>(false, ex.Message);
                }
            }
        }
    }
}
