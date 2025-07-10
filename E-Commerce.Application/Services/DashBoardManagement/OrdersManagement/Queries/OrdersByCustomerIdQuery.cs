using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
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
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Queries
{
    public class OrdersByCustomerIdQuery
    {
        public class Query : IRequest<BaseResponse<List<OrderModel>>>
        {
            public Guid CustomerId { get; set; }
            public Query(Guid customerId)
            {
                CustomerId = customerId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Query, BaseResponse<List<OrderModel>>>
        {
            public async Task<BaseResponse<List<OrderModel>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    List<OrderModel> orders = _unitOfWork.GetRepository<Order>().GetByExpressionAsNoTracking(p => p.CustomerId == request.CustomerId).Select(s => new OrderModel()
                    {
                        CustomerId = s.CustomerId,
                        OrderId = s.OrderId,
                        OrderStatusId = s.OrderStatusId,
                        PaymentMethodId = s.PaymentMethodId,
                        OrderStatus = s.OrderStatus.Name,
                        PaymentMethod = s.PaymentMethod.Name,
                        OrderDetails = s.OrderDetails.Select(o => new OrderDetailModel()
                        {
                            OrderDetailId = o.OrderDetailId,
                            Discount = o.Discount,
                            OrderId = o.OrderId,
                            Price = o.Price,
                            Quantity = o.Quantity,
                            ShopItemId = o.ShopItemId,
                            Tax = o.Tax,
                            ItemName = o.ShopItem.Item.Name,
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
