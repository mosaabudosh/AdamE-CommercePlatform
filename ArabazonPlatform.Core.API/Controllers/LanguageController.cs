using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Services.Language.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LanguageController(IMediator _mediator) : BaseController
    {
        [Route("languages")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetLanguages()
        {
            try
            {
                return await _mediator.Send(new LanguagesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetLanguages), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
