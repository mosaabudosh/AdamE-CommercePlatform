using Adam.Generic.Generic;
using E_Commerce.Application.Services.Gender.Queries;
using E_Commerce.Domain.Domains;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
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
                return await _mediator.Send(new GendersQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetGenders), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
