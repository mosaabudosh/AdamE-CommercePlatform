using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Model.Notification;
using ArabazonPlatform.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using ArabazonPlatform.Application.Services.NotificationManagement.Notification.Commands;
using ArabazonPlatform.Application.Services.NotificationManagement.Notification.Queries;
using ArabazonPlatform.Application.Services.NotificationManagement.NotificationStatusManagement.Commands;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationManagementController(IMediator _mediator) : BaseController
    {

        [HttpGet]
        [Route("notification-count-by-customer-id/{customerId}")]
        public async Task<ActionResult<BaseResponse<int>>> GetNotificationCountByCustomerId(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new NotificationCountQueryByCustomerId.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationCountByCustomerId), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("notifications-by-customer-id/{customerId}")]
        public async Task<ActionResult<BaseResponse<List<NotificationModel>>>> GetNotificationsByCustomerId(Guid customerId)
        {
            try
            {
                return await _mediator.Send(new NotificationsQueryByCustomerId.Query(customerId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationsByCustomerId), UserAccessor.GetCurrentUserId(), customerId);
                return new BaseResponse<List<NotificationModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("notification-count-by-user-id/{userId}")]
        public async Task<ActionResult<BaseResponse<int>>> GetNotificationCountByUserId(Guid userId)
        {
            try
            {
                return await _mediator.Send(new NotificationCountQueryByUserId.Query(userId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationCountByUserId), UserAccessor.GetCurrentUserId(), userId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("notifications-by-user-id/{userId}")]
        public async Task<ActionResult<BaseResponse<List<NotificationModel>>>> GetNotificationsByUserId(Guid userId)
        {
            try
            {
                return await _mediator.Send(new NotificationsQueryByUserId.Query(userId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationsByUserId), UserAccessor.GetCurrentUserId(), userId);
                return new BaseResponse<List<NotificationModel>>(false, ex.Message);
            }
        }


        [HttpPost]
        [Route("notifications-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateNotifications([FromBody] List<NotificationUpdationModel> notificationUpdationModels)
        {
            try
            {
                return await _mediator.Send(new NotificationUpdationCommands.Command(notificationUpdationModels));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateNotifications), UserAccessor.GetCurrentUserId(), notificationUpdationModels);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
