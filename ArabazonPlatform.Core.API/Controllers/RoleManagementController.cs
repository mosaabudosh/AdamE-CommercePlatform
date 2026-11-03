using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.AuthenticationManagement.RolesManagement.Commands;
using ArabazonPlatform.Application.Services.AuthenticationManagement.RolesManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("role-creation")]
        public async Task<ActionResult<BaseResponse>> AddRole([FromBody] RoleModel roleModel)
        {
            try
            {
                return await _mediator.Send(new RoleCreationCommands.Command(roleModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AddRole), UserAccessor.GetCurrentUserId(), roleModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("role-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateRole([FromBody] RoleModel roleModel)
        {
            try
            {
                return await _mediator.Send(new RoleUpdationCommands.Command(roleModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateRole), UserAccessor.GetCurrentUserId(), roleModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("role-by-id/{roleId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<RoleModel>>> GetRoleById(int roleId)
        {
            try
            {
                return await _mediator.Send(new RoleQueryById.Query(roleId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRoleById), UserAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse<RoleModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("role-deletion/{roleId}")]
        public async Task<ActionResult<BaseResponse>> DeleteRole(int roleId)
        {
            try
            {
                return await _mediator.Send(new RoleDeletionCommands.Command(roleId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteRole), UserAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("roles")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<RoleModel>>>> GetRoles(int pageNumber, int pageSize,string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new RolesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRoles), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<RoleModel>>(false, ex.Message);
            }
        }

        [Route("roles-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetRolesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<RoleModel>> responseRoles = await _mediator.Send(new RolesQuery.Query(0, int.MaxValue, searchText));
                if (!responseRoles.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseRoles.Message, responseRoles.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, responseRoles.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRolesCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("roles-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetRolesDDl()
        {
            try
            {
                return await _mediator.Send(new RolesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRolesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
