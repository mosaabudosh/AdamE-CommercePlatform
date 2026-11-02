using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.AuthenticationManagement.CustomersManagement.Queries;
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

        public class Handler(
            IUnitOfWork _unitOfWork,
            IUserAccessor _userAccessor,
            IExceptionService _exceptionService,
            IMediator _mediator
        ) : IRequestHandler<Command, BaseResponse<OrderModel>>
        {
            public async Task<BaseResponse<OrderModel>> Handle(
                Command request,
                CancellationToken cancellationToken)
            {
                try
                {
                    // ─── Step 1: Validate stock for every order line ──────────────────────
                    List<Guid> shopItemIds = request.OrderModel.OrderDetails
                        .Select(d => d.ShopItemId)
                        .Distinct()
                        .ToList();

                    IQueryable<ShopItem> shopItemsQuery = _unitOfWork
                        .GetRepository<ShopItem>()
                        .GetByExpression(si => shopItemIds.Contains(si.ShopItemId) && si.IsActive);

                    List<ShopItem> shopItems = shopItemsQuery.ToList();

                    // Map for quick lookup
                    Dictionary<Guid, ShopItem> shopItemMap = shopItems
                        .ToDictionary(si => si.ShopItemId);

                    foreach (var detail in request.OrderModel.OrderDetails)
                    {
                        if (!shopItemMap.TryGetValue(detail.ShopItemId, out ShopItem? item))
                        {
                            return new BaseResponse<OrderModel>(
                                false,
                                $"Product with ID '{detail.ShopItemId}' was not found or is no longer available.",
                                ErrorCode.NotFound);
                        }

                        if (item.IsBlock)
                        {
                            return new BaseResponse<OrderModel>(
                                false,
                                $"Product '{item.Name}' is currently blocked and cannot be ordered.",
                                ErrorCode.ValidationError);
                        }

                        double availableStock = (item.InStocks ?? 0) - (item.OutOfStocks ?? 0);

                        if (availableStock <= 0)
                        {
                            return new BaseResponse<OrderModel>(
                                false,
                                $"Product '{item.Name}' is out of stock.",
                                ErrorCode.ValidationError);
                        }

                        if (detail.Quantity > availableStock)
                        {
                            return new BaseResponse<OrderModel>(
                                false,
                                $"Requested quantity ({detail.Quantity}) for '{item.Name}' " +
                                $"exceeds available stock ({availableStock}).",
                                ErrorCode.ValidationError);
                        }
                    }

                    // ─── Step 2: Build the Order entity ──────────────────────────────────
                    Order order = new Order()
                    {
                        CustomerId = request.OrderModel.CustomerId,
                        OrderStatusId = request.OrderModel.OrderStatusId,
                        PaymentMethodId = request.OrderModel.PaymentMethodId,
                        CustomerCardInfoId = request.OrderModel.CustomerCardInfoId,
                        CustomerDeliveryAddressId = request.OrderModel.CustomerDeliveryAddressId,
                        DiscountCode = request.OrderModel.DiscountCode,
                        ReferenceNumber = request.OrderModel.ReferenceNumber,
                        CreationDate = DateTime.UtcNow,
                        IsActive = true,
                        OrderDetails = request.OrderModel.OrderDetails.Select(s => new OrderDetail()
                        {
                            ShopItemId = s.ShopItemId,
                            Discount = s.Discount,
                            Price = s.Price,
                            Quantity = s.Quantity,
                            Tax = s.Tax,
                            ColorId = s.ColorId,
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        }).ToList(),
                    };

                    order = GenerateReferenceNumber(order);
                    _unitOfWork.GetRepository<Order>().Insert(order);

                    // ─── Step 3: Deduct stock (increment OutOfStocks) ─────────────────────
                    // We update OutOfStocks so that InStocks remains the total received,
                    // and the formula "available = InStocks - OutOfStocks" stays consistent.
                    foreach (var detail in request.OrderModel.OrderDetails)
                    {
                        ShopItem item = shopItemMap[detail.ShopItemId];
                        item.OutOfStocks = (item.OutOfStocks ?? 0) + detail.Quantity;
                        item.LastUpdatedDate = DateTime.UtcNow;
                        item.LastUpdateUserId = _userAccessor.GetCurrentUserId();
                        _unitOfWork.GetRepository<ShopItem>().Update(item);
                    }

                    await _unitOfWork.CommitChanges();

                    // ─── Step 4: Notifications ────────────────────────────────────────────
                    List<NotificationModel> notificationModels = new List<NotificationModel>();

                    BaseResponse<CustomerModel> responseCustomer =
                        await _mediator.Send(new GetCustomerByIdQuery.Query(request.OrderModel.CustomerId));

                    if (!responseCustomer.IsSuccess)
                        return new BaseResponse<OrderModel>(false, responseCustomer.Message, responseCustomer.ErrorCode, null);

                    BaseResponse<List<Guid>> responseUsersByShopItemId =
                        await _mediator.Send(new GetUsersByShopItemIdQuery.Query(
                            request.OrderModel.OrderDetails.FirstOrDefault().ShopItemId));

                    if (!responseUsersByShopItemId.IsSuccess)
                        return new BaseResponse<OrderModel>(false, responseUsersByShopItemId.Message, responseUsersByShopItemId.ErrorCode, null);

                    notificationModels.AddRange(responseUsersByShopItemId.DataObject.Select(s =>
                        new NotificationModel()
                        {
                            Subject = "Received Order",
                            SubjectAr = "استلام الطلب",
                            Description = $"Received order from customer",
                            DescriptionAr = $"تم استلام الطلب من العميل",
                            NotificationCode = NotificationTypes.CreateOrder.ToString(),
                            NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                            NotificationTypeId = (int)Enums.NotificationTypes.CreateOrder,
                            Tooltip = "Received Order",
                            TooltipAr = "استلام الطلب",
                            UserId = s,
                        }));

                    notificationModels.Add(new NotificationModel()
                    {
                        Subject = "Send Order",
                        SubjectAr = "إرسال الطلب",
                        Description = "Purchase order has been sent",
                        DescriptionAr = "تم ارسال طلب الشراء",
                        NotificationCode = NotificationTypes.CreateOrder.ToString(),
                        NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                        NotificationTypeId = (int)Enums.NotificationTypes.CreateOrder,
                        Tooltip = "Send Order",
                        TooltipAr = "إرسال الطلب",
                        CustomerId = request.OrderModel.CustomerId
                    });

                    BaseResponse responseNotification =
                        await _mediator.Send(new NotificationCreationCommands.Command(notificationModels));

                    if (!responseNotification.IsSuccess)
                        return new BaseResponse<OrderModel>(false, responseNotification.Message, responseNotification.ErrorCode, null);

                    // ─── Step 5: Build response model ─────────────────────────────────────
                    OrderModel createdOrderModel = new OrderModel()
                    {
                        CustomerId = order.CustomerId,
                        OrderStatusId = order.OrderStatusId,
                        PaymentMethodId = order.PaymentMethodId,
                        OrderId = order.OrderId,
                        CustomerCardInfoId = order.CustomerCardInfoId,
                        CustomerDeliveryAddressId = order.CustomerDeliveryAddressId,
                        DiscountCode = order.DiscountCode,
                        OrderDate = order.CreationDate,
                        ReferenceNumber = order.ReferenceNumber,
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

                    return new BaseResponse<OrderModel>(true, string.Empty, createdOrderModel);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null) ex = ex.InnerException;
                    await _exceptionService.LogErrorAsync(ex, "OrderCreationCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<OrderModel>(false, ex.Message);
                }
            }

            /// <summary>
            /// Generates a unique 15-digit reference number for the order
            /// using a Fischer-Yates shuffle on digits 0-14.
            /// </summary>
            private static Order GenerateReferenceNumber(Order order)
            {
                int[] numbers = Enumerable.Range(0, 15).ToArray();
                Random random = new Random();

                for (int i = numbers.Length - 1; i > 0; i--)
                {
                    int j = random.Next(i + 1);
                    (numbers[i], numbers[j]) = (numbers[j], numbers[i]);
                }

                order.ReferenceNumber = string.Join(string.Empty, numbers);
                return order;
            }
        }
    }
}
