using Adam.Generic.Generic;
using E_Commerce.Application.Services.Country.Queries;
using E_Commerce.Application.Services.Shop.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopController(IMediator _mediator) : BaseController
    {
        [Route("shops")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCountries()
        {
            try
            {
                return await _mediator.Send(new ShopQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCountries), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
