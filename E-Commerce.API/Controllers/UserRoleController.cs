using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Services.AuthenticationManagement.UserRolesManagement.Commands;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("user-roles-creation")]
        public async Task<ActionResult<BaseResponse>> CreateUserRoles([FromBody] List<UserRoleModel> userRoleModels)
        {
            try
            {
                return await _mediator.Send(new UserRoleCreationCommands.Command(userRoleModels));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateUserRoles), UserAccessor.GetCurrentUserId(), userRoleModels);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("user-role-deletion/{userRoleId}")]
        public async Task<ActionResult<BaseResponse>> DeleteUserRole(int userRoleId)
        {
            try
            {
                return await _mediator.Send(new UserRoleDeletionCommands.Command(userRoleId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteUserRole), UserAccessor.GetCurrentUserId(), userRoleId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
