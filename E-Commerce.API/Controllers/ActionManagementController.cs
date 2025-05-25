using Adam.Generic.Generic;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.Action.Queries;
using E_Commerce.Application.Services.City.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActionManagementController : BaseController
    {
        #region Fields
        private readonly IActionManagementService _userManagementService;
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public ActionManagementController(IActionManagementService actionManagementService, IMediator mediator)
        {
            _userManagementService = actionManagementService;
            _mediator = mediator;
        }
        #endregion

        [HttpPost]
        [Route("action-creation")]
        public async Task<ActionResult<BaseResponse>> AddAction([FromBody] ActionModel ActionModel)
        {
            try
            {
                return await _userManagementService.AddAction(ActionModel);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AddAction), UserAccessor.GetCurrentUserId(), ActionModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("action-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateAction([FromBody] ActionModel actionModel)
        {
            try
            {
                return await _userManagementService.UpdateAction(actionModel);
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
                return await _userManagementService.GetActionById(actionId);
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
                return await _userManagementService.DeleteAction(actionId);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteAction), UserAccessor.GetCurrentUserId(), actionId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("actions")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ActionModel>>>> GetActions(int pageNumber, int pageSize)
        {
            try
            {
                return await _userManagementService.GetActions(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActions), UserAccessor.GetCurrentUserId(), pageNumber, pageSize);
                return new BaseResponse<List<ActionModel>>(false, ex.Message);
            }
        }

        [Route("actions-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetActionsCount()
        {
            try
            {
                return await _userManagementService.GetActionsCount();
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActionsCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("actions-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetActionsDDl()
        {
            try
            {
                return await _mediator.Send(new ActionQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetActionsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
