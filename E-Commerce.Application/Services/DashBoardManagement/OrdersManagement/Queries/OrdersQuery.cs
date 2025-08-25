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

namespace E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Queries
{
    public class OrdersQuery
    {
        public class Query : IRequest<BaseResponse<List<OrderModel>>>
        {
            public Guid? CustomerId { get; set; }
            public Guid? ShopItemId { get; set; }
            public Guid? UserId { get; set; }
            public int? CategoryId { get; set; }
            public int? OrderStatusId { get; set; }
            public string? SearchText { get; set; }

            public Query(Guid? customerId = null, Guid? shopItemId = null, Guid? userId = null, int? categoryId = null, int? orderStatusId = null, string? searchText = null)
            {
                CustomerId = customerId;
                ShopItemId = shopItemId;
                UserId = userId;
                CategoryId = categoryId;
                OrderStatusId = orderStatusId;
                SearchText = searchText;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<OrderModel>>>
        {
            public async Task<BaseResponse<List<OrderModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<OrderModel> orders = _unitOfWork.GetRepository<Order>().GetByExpressionAsNoTracking(p => (!request.CustomerId.HasValue || p.CustomerId == request.CustomerId) && (!request.UserId.HasValue || p.LastUpdateUserId == request.UserId) && (!request.OrderStatusId.HasValue || p.OrderStatusId == request.OrderStatusId) && (!request.ShopItemId.HasValue || p.OrderDetails.Any(o => o.ShopItemId == request.ShopItemId)) && (!request.CategoryId.HasValue || p.OrderDetails.Any(o => o.ShopItem.CategoryId == request.CategoryId))).Select(s => new OrderModel()
                    {
                        CustomerId = s.CustomerId,
                        OrderId = s.OrderId,
                        OrderStatusId = s.OrderStatusId,
                        PaymentMethodId = s.PaymentMethodId,
                        OrderStatus = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.OrderStatus.Name : s.OrderStatus.NameAr,
                        Customer = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.Customer.FirstName} {s.Customer.LastName}" : $"{s.LastUpdateUser.FirstNameAr} {s.Customer.LastNameAr}",
                        OrderDate = s.CreationDate,
                        UpdateByUser = s.LastUpdateUserId.HasValue ? (_userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{s.LastUpdateUser.FirstName} {s.LastUpdateUser.LastName}" : $"{s.LastUpdateUser.FirstNameAr} {s.LastUpdateUser.LastNameAr}") : null,
                        PaymentMethod = _userAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? s.PaymentMethod.Name : s.PaymentMethod.NameAr,
                        OrderDetails = s.OrderDetails.Select(o => new OrderDetailModel()
                        {
                            OrderDetailId = o.OrderDetailId,
                            Discount = o.Discount,
                            OrderId = o.OrderId,
                            Price = o.Price,
                            Quantity = o.Quantity,
                            ShopItemId = o.ShopItemId,
                            Tax = o.Tax,
                            ItemName = o.ShopItem.Name,
                            Shope = o.ShopItem.Shop.Name
                        }).ToList()
                    }).ToList();

                    return new BaseResponse<List<OrderModel>>(true, string.Empty, string.Empty, orders);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrdersByCustomerIdQuery", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<OrderModel>>(false, ex.Message);
                }
            }
        }
    }
}
