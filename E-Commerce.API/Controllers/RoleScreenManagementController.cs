using E_Commerce.Application.Model;
using E_Commerce.Application.Services.City.Commands;
using E_Commerce.Application.Services.City.Queries;
using E_Commerce.Application.Services.RoleScreen.Queries;
using E_Commerce.Application.Services.RoleScreenManagement.Commands;
using E_Commerce.Application.Services.RoleScreenManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleScreenManagementController(IMediator _mediator) : BaseController
    {

        [Route("assign-un-assign-screens-for-role-by-role-id/{roleId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<AssignUnAssignScreensForRoleModel>>> AssignUnAssignScreensForRoleByRoleId(int roleId)
        {
            try
            {
                return await _mediator.Send(new AssignUnAssignScreensForRoleByRoleIdQuery.Query(roleId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AssignUnAssignScreensForRoleByRoleId), UserAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse<AssignUnAssignScreensForRoleModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("creation-updation-role-screens")]
        public async Task<ActionResult<BaseResponse>> CreationUpdationRoleScreens([FromBody] CreationUpdationRoleScreenModel creationUpdationRoleScreenModel)
        {
            try
            {
                return await _mediator.Send(new CreationUpdationRoleScreenCommands.Command(creationUpdationRoleScreenModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreationUpdationRoleScreens), UserAccessor.GetCurrentUserId(), creationUpdationRoleScreenModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("role-screen-actions-by-screen-id/{screenId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<int>>>> GetRoleScreenActionsByScreenId(int screenId)
        {
            try
            {
                return await _mediator.Send(new RoleScreenActionsByScreenIdQuery.Query(screenId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRoleScreenActionsByScreenId), UserAccessor.GetCurrentUserId(), screenId);
                return new BaseResponse<List<int>>(false, ex.Message);
            }
        }

    }
}
