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

namespace E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Commands
{
    public class OrderCreationCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public OrderModel OrderModel { get; set; }
            public Command(OrderModel orderModel)
            {
                OrderModel = orderModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
        {
            public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Order order = new Order()
                    {
                        CustomerId = request.OrderModel.CustomerId,
                        OrderStatusId = request.OrderModel.OrderStatusId,
                        PaymentMethodId = request.OrderModel.PaymentMethodId,
                        CreationUserId = _userAccessor.GetCurrentUserId(),
                        CreationDate = DateTime.UtcNow,
                        IsActive = true,
                        OrderDetails = request.OrderModel.OrderDetails.Select(s => new OrderDetail()
                        {
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Quantity = s.Quantity,
                            Tax = s.Tax,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        }).ToList(),
                    };
                  
                    _unitOfWork.GetRepository<Order>().Insert(order);
                    await _unitOfWork.CommitChanges();
                    return new BaseResponse(true);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse(false, ex.Message);
                }
            }

            
        }
    }
}
