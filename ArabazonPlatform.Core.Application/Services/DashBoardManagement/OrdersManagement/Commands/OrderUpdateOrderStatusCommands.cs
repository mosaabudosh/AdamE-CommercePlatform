using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.AuthenticationManagement.CustomersManagement.Queries;
using ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using ArabazonPlatform.Application.Services.NotificationManagement.Notification.Commands;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.OrdersManagement.Commands
{
    public class OrderUpdateOrderStatusCommands
    {
        public class Command : IRequest<BaseResponse<List<NotificationModel>>>
        {
            public UpdateOrderStatusModel UpdateOrderStatusModel { get; set; }
            public Command(UpdateOrderStatusModel updateOrderStatusModel)
            {
                UpdateOrderStatusModel = updateOrderStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse<List<NotificationModel>>>
        {
            public async Task<BaseResponse<List<NotificationModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    Order exsistOrder = _unitOfWork.GetRepository<Order>().GetByExpressionAsNoTracking(p => p.OrderId == request.UpdateOrderStatusModel.OrderId).SingleOrDefault();

                    exsistOrder.OrderStatusId = request.UpdateOrderStatusModel.NewOrderStatusId;
                    exsistOrder.LastUpdatedDate = DateTime.UtcNow;
                    exsistOrder.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    _unitOfWork.GetRepository<Order>().Update(exsistOrder);
                    await _unitOfWork.CommitChanges();

                    List<NotificationModel> notificationModels = new List<NotificationModel>();
                    NotificationModel notificationModel = new NotificationModel();
                    switch (request.UpdateOrderStatusModel.NewOrderStatusId)
                    {
                        case (int)OrderStatuses.Approved:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order Approved",
                                SubjectAr = "تمت الموافقة على الطلب",
                                Description = "Order Was Approved ",
                                DescriptionAr = "تمت الموافقة على الطلب ",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order Approved",
                                TooltipAr = "تمت الموافقة على الطلب",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;

                        case (int)OrderStatuses.Rejected:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order Rejected",
                                SubjectAr = "تم رفض الطلب",
                                Description = "Order Was Rejected ",
                                DescriptionAr = "تم رفض الطلب",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order Rejected",
                                TooltipAr = "تم رفض الطلب",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;

                        case (int)OrderStatuses.InProgrees:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order In Progrees",
                                SubjectAr = "الطلب قيد التقدم",
                                Description = "Order In Progrees ",
                                DescriptionAr = "الطلب قيد التقدم",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order In Progrees",
                                TooltipAr = "الطلب قيد التقدم",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;

                        case (int)OrderStatuses.InDelivery:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order In Delivery",
                                SubjectAr = "الطلب قيد التوصيل",
                                Description = "Order In Delivery ",
                                DescriptionAr = "الطلب قيد التوصيل",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order In Delivery",
                                TooltipAr = "الطلب قيد التوصيل",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;

                        case (int)OrderStatuses.Delivered:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order Delivered",
                                SubjectAr = "تم تسليم الطلب",
                                Description = "Order Delivered",
                                DescriptionAr = "تم تسليم الطلب",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order Delivered",
                                TooltipAr = "تم تسليم الطلب",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;

                        case (int)OrderStatuses.Refund:
                            notificationModel = new NotificationModel()
                            {
                                Subject = "Order Refund",
                                SubjectAr = "استرداد الطلب",
                                Description = "Order Refund",
                                DescriptionAr = "استرداد الطلب",
                                NotificationCode = NotificationTypes.ChangeOrderStatus.ToString(),
                                NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                NotificationTypeId = (int)Enums.NotificationTypes.ChangeOrderStatus,
                                Tooltip = "Order Refund",
                                TooltipAr = "استرداد الطلب",
                                CustomerId = exsistOrder.CustomerId
                            };
                            break;
                    }
                    notificationModels.Add(notificationModel);

                    BaseResponse responseNotificationCreation = await _mediator.Send(new NotificationCreationCommands.Command(notificationModels));
                    if (!responseNotificationCreation.IsSuccess)
                    {
                        return new BaseResponse<List<NotificationModel>>(false, responseNotificationCreation.Message, responseNotificationCreation.ErrorCode, null);
                    }
                    return new BaseResponse<List<NotificationModel>>(true, string.Empty, notificationModels);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "OrderUpdateOrderStatusCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<NotificationModel>>(false, ex.Message);
                }
            }


        }
    }
}
