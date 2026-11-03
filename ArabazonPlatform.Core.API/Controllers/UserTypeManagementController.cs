using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Services.AuthenticationManagement.UserTypesManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserTypeManagementController(IMediator _mediator) : BaseController
    {
        [Route("user-types")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetUserTypes()
        {
            try
            {
                return await _mediator.Send(new UserTypesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetUserTypes), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
