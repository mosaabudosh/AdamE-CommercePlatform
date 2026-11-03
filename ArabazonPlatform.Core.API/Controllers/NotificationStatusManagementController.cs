using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model;
using ArabazonPlatform.Application.Model.Notification;
using ArabazonPlatform.Application.Services.NotificationManagement.NotificationStatusManagement.Commands;
using ArabazonPlatform.Application.Services.NotificationManagement.NotificationStatusManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationStatusManagementController(IMediator _mediator) : BaseController
    {
        [Route("notification-statuses")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<NotificationStatusModel>>>> GetNotificationStatuss(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new NotificationStatusesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationStatuss), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<NotificationStatusModel>>(false, ex.Message);
            }
        }

        [Route("notification-statuses-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetNotificationStatusesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<NotificationStatusModel>> notificationStatusesResponse = await _mediator.Send(new NotificationStatusesQuery.Query(0, int.MaxValue, searchText));
                if (!notificationStatusesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, notificationStatusesResponse.Message, notificationStatusesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, notificationStatusesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationStatusesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("notification-status-by-id/{notificationStatusId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<NotificationStatusModel>>> NotificationStatusById(int notificationStatusId)
        {
            try
            {
                return await _mediator.Send(new NotificationStatusQueryById.Query(notificationStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(NotificationStatusById), UserAccessor.GetCurrentUserId(), notificationStatusId);
                return new BaseResponse<NotificationStatusModel>(false, ex.Message);
            }
        }

        [Route("notification-statuses-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetNotificationStatusesDDl()
        {
            try
            {
                return await _mediator.Send(new NotificationStatusesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationStatusesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-status-creation")]
        public async Task<ActionResult<BaseResponse>> CreateNotificationStatus([FromBody] NotificationStatusModel notificationStatusModel)
        {
            try
            {
                return await _mediator.Send(new NotificationStatusCreationCommands.Command(notificationStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateNotificationStatus), UserAccessor.GetCurrentUserId(), notificationStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-status-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateNotificationStatus([FromBody] NotificationStatusModel notificationStatusModel)
        {
            try
            {
                return await _mediator.Send(new NotificationStatusUpdationCommands.Command(notificationStatusModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateNotificationStatus), UserAccessor.GetCurrentUserId(), notificationStatusModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-status-deletion/{notificationStatusId}")]
        public async Task<ActionResult<BaseResponse>> DeleteNotificationStatus(int notificationStatusId)
        {
            try
            {
                return await _mediator.Send(new NotificationStatusDeletionCommands.Command(notificationStatusId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteNotificationStatus), UserAccessor.GetCurrentUserId(), notificationStatusId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
