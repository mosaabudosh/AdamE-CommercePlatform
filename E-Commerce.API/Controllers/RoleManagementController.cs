using Adam.Generic.Generic;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.Action.Queries;
using E_Commerce.Application.Services.Role.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleManagementController : BaseController
    {
        #region Fields
        private readonly IRoleManagementService _roleManagementService;
        private readonly IMediator _mediator;
        #endregion

        #region Constructor
        public RoleManagementController(IRoleManagementService roleManagementService, IMediator mediator)
        {
            _roleManagementService = roleManagementService;
            _mediator = mediator;
        }
        #endregion

        [HttpPost]
        [Route("role-creation")]
        public async Task<ActionResult<BaseResponse>> AddRole([FromBody] RoleModel roleModel)
        {
            try
            {
                return await _roleManagementService.AddRole(roleModel);
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
                return await _roleManagementService.UpdateRole(roleModel);
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
                return await _roleManagementService.GetRoleById(roleId);
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
                return await _roleManagementService.DeleteRole(roleId);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteRole), UserAccessor.GetCurrentUserId(), roleId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [Route("roles")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<RoleModel>>>> GetRoles(int pageNumber, int pageSize)
        {
            try
            {
                return await _roleManagementService.GetRoles(pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRoles), UserAccessor.GetCurrentUserId(), pageNumber, pageSize);
                return new BaseResponse<List<RoleModel>>(false, ex.Message);
            }
        }

        [Route("roles-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetRolesCount()
        {
            try
            {
                return await _roleManagementService.GetRolesCount();
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRolesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("roles-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetRolesDDl()
        {
            try
            {
                return await _mediator.Send(new RoleQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetRolesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
