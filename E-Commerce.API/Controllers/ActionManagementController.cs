using Adam.Generic.Generic;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ActionModel = E_Commerce.Application.Model.Authorisation.ActionModel;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("action-creation")]
        public async Task<ActionResult<BaseResponse>> AddAction([FromBody] ActionModel actionModel)
        {
            try
            {
                return await _mediator.Send(new ActionCreationCommands.Command(actionModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AddAction), UserAccessor.GetCurrentUserId(), actionModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("action-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateAction([FromBody] ActionModel actionModel)
        {
            try
            {
                return await _mediator.Send(new ActionUpdationCommands.Command(actionModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateAction), UserAccessor.GetCurrentUserId(), actionModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("action-by-id/{actionId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ActionModel>>> GetActionById(int actionId)
        {
            try
            {
                return await _mediator.Send(new ActionQueryById.Query(actionId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActionById), UserAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse<ActionModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("action-deletion/{actionId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAction(int actionId)
        {
            try
            {
                return await _mediator.Send(new ActionDeletionCommands.Command(actionId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteAction), UserAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("actions")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ActionModel>>>> GetActions(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ActionsQuery.Query(pageNumber, pageSize,searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActions), UserAccessor.GetCurrentUserId(), pageNumber, pageSize,searchText);
                return new BaseResponse<List<ActionModel>>(false, ex.Message);
            }
        }

        [Route("actions-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetActionsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<ActionModel>> responseActionModels = await _mediator.Send(new ActionsQuery.Query(0, int.MaxValue, searchText));
                if (!responseActionModels.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseActionModels.Message, responseActionModels.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, responseActionModels.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActionsCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("actions-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetActionsDDl()
        {
            try
            {
                return await _mediator.Send(new ActionsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActionsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
