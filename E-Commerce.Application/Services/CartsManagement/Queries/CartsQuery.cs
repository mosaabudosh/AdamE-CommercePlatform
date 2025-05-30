using Adam.Logging.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services.Carts.Queries
{
    public class CartsQuery
    {
        public class Query : IRequest<BaseResponse<List<CartModel>>>
        {
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
            public Guid? UserId { get; set; }
            public Guid? TempCustomerId { get; set; }
            public int? CartStatusId { get; set; }
            public Guid? ShopItemId { get; set; }

            public Query(int pageNumber, int pageSize, Guid? userId = null, Guid? tempCustomerId = null, int? cartStatusId = null, Guid? shopItemId = null)
            {
                this.PageNumber = pageNumber;
                this.PageSize = pageSize;
                this.UserId = userId;
                this.TempCustomerId = tempCustomerId;
                this.CartStatusId = cartStatusId;
                this.ShopItemId = shopItemId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<CartModel>>>
        {
            public async Task<BaseResponse<List<CartModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<CartModel> cart = _unitOfWork.GetRepository<Domain.Domains.Cart>().GetByExpressionAsNoTracking(p => (!request.UserId.HasValue || p.UserId == request.UserId) && (!request.TempCustomerId.HasValue || p.TempCustomerId == request.TempCustomerId) && (!request.CartStatusId.HasValue || p.CartStatusId == request.CartStatusId) && (!request.ShopItemId.HasValue || p.CartDetails.Any(x => x.ShopItemId == request.ShopItemId))).Select(s => new CartModel()
                    {
                        CartId = s.CartId,
                        UserId = s.UserId,
                        CartStatusId = s.CartStatusId,
                        TempCustomerId = s.TempCustomerId,
                        CartStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.CartStatus.Name : s.CartStatus.NameAr,
                        TempCustomer = !s.TempCustomerId.HasValue ? null : _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.TempCustomer.FirstName} {s.TempCustomer.SecondName} {s.TempCustomer.LastName}" : $"{s.TempCustomer.FirstNameAr} {s.TempCustomer.SecondNameAr} {s.TempCustomer.LastNameAr}",
                        User = !s.UserId.HasValue ? null : _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.User.FirstName} {s.User.SecondName} {s.User.LastName}" : $"{s.User.FirstNameAr} {s.User.SecondNameAr} {s.User.LastNameAr}",
                        CartDetailModels = s.CartDetails.Select(p => new CartDetailModel()
                        {
                            CartDetailId = p.CartDetailId,
                            Discount = p.Discount,
                            Price = p.Price,
                            Quantity = p.Quantity,
                            ShopItemId = p.ShopItemId,
                            Tax = p.Tax,
                            ItemName = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? p.ShopItem.Item.Name : p.ShopItem.Item.NameAr
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
