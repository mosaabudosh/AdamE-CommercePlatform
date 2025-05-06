using Adam.Generic.Generic;
using E_Commerce.Application.Services.Language.Queries;
using E_Commerce.Application.Services.UserType.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
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
                return await _mediator.Send(new LanguageQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetLanguages), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
