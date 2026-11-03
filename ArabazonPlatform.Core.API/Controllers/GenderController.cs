using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Services.Gender.Queries;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController(IMediator _mediator) : BaseController
    {
        [Route("genders")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetGenders()
        {
            try
            {
                return await _mediator.Send(new GendersDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetGenders), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
