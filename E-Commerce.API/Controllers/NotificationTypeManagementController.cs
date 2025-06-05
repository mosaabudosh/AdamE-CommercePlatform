using Adam.Generic.Generic;
using E_Commerce.Application.Model.Notification;
using E_Commerce.Application.Services.NotificationManagement.NotificationTypeManagement.Commands;
using E_Commerce.Application.Services.NotificationManagement.NotificationTypeManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationTypeManagementController(IMediator _mediator) : BaseController
    {
        [Route("notification-types")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<NotificationTypeModel>>>> GetNotificationTypes(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new NotificationTypesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationTypes), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<NotificationTypeModel>>(false, ex.Message);
            }
        }

        [Route("notification-types-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetNotificationTypesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<NotificationTypeModel>> notificationTypesResponse = await _mediator.Send(new NotificationTypesQuery.Query(0, int.MaxValue, searchText));
                if (!notificationTypesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, notificationTypesResponse.Message, notificationTypesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, notificationTypesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationTypesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("notification-type-by-id/{notificationTypeId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<NotificationTypeModel>>> NotificationTypeById(int notificationTypeId)
        {
            try
            {
                return await _mediator.Send(new NotificationTypeQueryById.Query(notificationTypeId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(NotificationTypeById), UserAccessor.GetCurrentUserId(), notificationTypeId);
                return new BaseResponse<NotificationTypeModel>(false, ex.Message);
            }
        }

        [Route("notification-types-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetNotificationTypesDDl()
        {
            try
            {
                return await _mediator.Send(new NotificationTypesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetNotificationTypesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-type-creation")]
        public async Task<ActionResult<BaseResponse>> CreateNotificationType([FromBody] NotificationTypeModel notificationTypeModel)
        {
            try
            {
                return await _mediator.Send(new NotificationTypeCreationCommands.Command(notificationTypeModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateNotificationType), UserAccessor.GetCurrentUserId(), notificationTypeModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-type-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateNotificationType([FromBody] NotificationTypeModel notificationTypeModel)
        {
            try
            {
                return await _mediator.Send(new NotificationTypeUpdationCommands.Command(notificationTypeModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateNotificationType), UserAccessor.GetCurrentUserId(), notificationTypeModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("notification-type-deletion/{notificationTypeId}")]
        public async Task<ActionResult<BaseResponse>> DeleteNotificationType(int notificationTypeId)
        {
            try
            {
                return await _mediator.Send(new NotificationTypeDeletionCommands.Command(notificationTypeId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteNotificationType), UserAccessor.GetCurrentUserId(), notificationTypeId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
