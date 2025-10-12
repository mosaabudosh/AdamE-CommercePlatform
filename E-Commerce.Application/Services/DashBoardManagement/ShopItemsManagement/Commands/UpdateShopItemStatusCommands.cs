using Adam.Logging.Interfaces;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using E_Commerce.Application.Services.NotificationManagement.Notification.Commands;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.Enum;
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

namespace E_Commerce.Application.Services.DashBoardManagement.ShopItemsManagement.Commands
{
    public class UpdateShopItemStatusCommands
    {
        public class Command : IRequest<BaseResponse<List<NotificationModel>>>
        {
            public UpdateShopItemStatusModel UpdateShopItemStatusModel { get; set; }
            public Command(UpdateShopItemStatusModel updateShopItemStatusModel)
            {
                UpdateShopItemStatusModel = updateShopItemStatusModel;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, IMediator _mediator) : IRequestHandler<Command, BaseResponse<List<NotificationModel>>>
        {
            public async Task<BaseResponse<List<NotificationModel>>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    ShopItem exsistShopItem = _unitOfWork.GetRepository<ShopItem>().GetByExpressionAsNoTracking(p => p.ShopItemId == request.UpdateShopItemStatusModel.ShopItemId).SingleOrDefault();

                    exsistShopItem.ShopItemStatusId = request.UpdateShopItemStatusModel.ShopItemStatusId;
                    exsistShopItem.LastUpdatedDate = DateTime.UtcNow;
                    exsistShopItem.LastUpdateUserId = _userAccessor.GetCurrentUserId();

                    _unitOfWork.GetRepository<ShopItem>().Update(exsistShopItem);
                    await _unitOfWork.CommitChanges();
                    BaseResponse<List<Guid>> responseUsersByShopItemId = await _mediator.Send(new GetUsersByShopItemIdQuery.Query(exsistShopItem.ShopItemId));
                    if (!responseUsersByShopItemId.IsSuccess)
                    {
                        return new BaseResponse<List<NotificationModel>>(false, responseUsersByShopItemId.Message, responseUsersByShopItemId.ErrorCode, null);
                    }
                    List<NotificationModel> notificationModels = new List<NotificationModel>();
                    NotificationModel notificationModel = new NotificationModel();
                    foreach (Guid userId in responseUsersByShopItemId.DataObject)
                    {
                        switch (request.UpdateShopItemStatusModel.ShopItemStatusId)
                        {
                            case (int)ShopItemStatuses.Approved:
                                notificationModel = new NotificationModel()
                                {
                                    Subject = "Item Approved",
                                    SubjectAr = "تمت الموافقة على السلعة",
                                    Description = "Item Was Approved ",
                                    DescriptionAr = "تمت الموافقة على السلعة ",
                                    NotificationCode = NotificationTypes.ChangeShopItemStatuses.ToString(),
                                    NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                    NotificationTypeId = (int)Enums.NotificationTypes.ChangeShopItemStatuses,
                                    Tooltip = "Item Approved",
                                    TooltipAr = "تمت الموافقة على السلعة",
                                    UserId = userId
                                };
                                break;

                            case (int)ShopItemStatuses.Rejected:
                                notificationModel = new NotificationModel()
                                {
                                    Subject = "Item Rejected",
                                    SubjectAr = "تم رفض السلعة",
                                    Description = "Item Was Rejected ",
                                    DescriptionAr = "تم رفض السلعة",
                                    NotificationCode = NotificationTypes.ChangeShopItemStatuses.ToString(),
                                    NotificationStatusId = (int)Enums.NotificationStatuses.UnRead,
                                    NotificationTypeId = (int)Enums.NotificationTypes.ChangeShopItemStatuses,
                                    Tooltip = "Item Rejected",
                                    TooltipAr = "تم رفض السلعة",
                                    CustomerId = userId
                                };
                                break;

                        }
                        notificationModels.Add(notificationModel);
                    }
                

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
                    await _exceptionService.LogErrorAsync(ex, "UpdateShopItemStatusCommands", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<List<NotificationModel>>(false, ex.Message);
                }
            }


        }
    }
}
