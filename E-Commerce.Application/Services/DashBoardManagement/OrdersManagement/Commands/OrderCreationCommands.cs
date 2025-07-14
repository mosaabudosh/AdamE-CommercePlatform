using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using E_Commerce.Application.Services.NotificationManagement;
using E_Commerce.Application.Services.NotificationManagement.Notification.Commands;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Adam.Generic.Constants.Constants;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.OrdersManagement.Commands
{
    public class OrderCreationCommands
    {
        public class Command : IRequest<BaseResponse<OrderModel>>
        {
            public OrderModel OrderModel { get; set; }
            public Command(OrderModel orderModel)
            {
                OrderModel = orderModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse<OrderModel>>
        {
            public async Task<BaseResponse<OrderModel>> Handle(Command request, CancellationToken cancellationToken)
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

                    List<NotificationModel> notificationModels = new List<NotificationModel>();

                    BaseResponse<List<Guid>> responseUsersByShopItemId = await _mediator.Send(new GetUsersByShopItemIdQuery.Query(request.OrderModel.OrderDetails.FirstOrDefault().ShopItemId));
                    if (!responseUsersByShopItemId.IsSuccess)
                    {
                        return new BaseResponse<OrderModel>(false, responseUsersByShopItemId.Message, responseUsersByShopItemId.ErrorCode, null);
                    }

                    notificationModels.AddRange(responseUsersByShopItemId.DataObject.Select(s => 
                    new NotificationModel()
                    {
                        Subject = "Send Order",
                        SubjectAr = "إرسال الطلب",
                        Description = "Purchase order has been sent",
                        DescriptionAr = "تم ارسال طلب الشراء ",
                        NotificationCode = NotificationTypes.CreateOrder.ToString(),
                        NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                        NotificationTypeId = (int)Enums.NotificationTypes.CreateOrder,
                        Tooltip = "Send Order",
                        TooltipAr = "إرسال الطلب",
                        UserId = s
                    }).ToList());

                    notificationModels.Add(new NotificationModel()
                    {
                        Subject = "Send Order",
                        SubjectAr = "إرسال الطلب",
                        Description = "Purchase order has been sent",
                        DescriptionAr = "تم ارسال طلب الشراء ",
                        NotificationCode = NotificationTypes.CreateOrder.ToString(),
                        NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                        NotificationTypeId = (int)Enums.NotificationTypes.CreateOrder,
                        Tooltip = "Send Order",
                        TooltipAr = "إرسال الطلب",
                        CustomerId = request.OrderModel.CustomerId
                    });


                    BaseResponse responseNotificationCreation = await _mediator.Send(new NotificationCreationCommands.Command(notificationModels));
                    if (!responseNotificationCreation.IsSuccess)
                    {
                        return new BaseResponse<OrderModel>(false, responseNotificationCreation.Message, responseNotificationCreation.ErrorCode, null);
                    }

                    OrderModel creationOrderModel = new OrderModel()
                    {
                        CustomerId = order.CustomerId,
                        OrderStatusId = order.OrderStatusId,
                        PaymentMethodId = order.PaymentMethodId,
                        OrderId = order.OrderId,
                        Notifications = notificationModels,
                        OrderDetails = order.OrderDetails.Select(s => new OrderDetailModel()
                        {
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Quantity = s.Quantity,
                            Tax = s.Tax,
                            OrderDetailId = s.OrderDetailId
                        }).ToList(),
                    };

                    return new BaseResponse<OrderModel>(true, string.Empty, creationOrderModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<OrderModel>(false, ex.Message);
                }
            }

            
        }
    }
}
