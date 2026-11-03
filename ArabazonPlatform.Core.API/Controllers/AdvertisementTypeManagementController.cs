using ArabazonPlatform.Application.Services.AdvertisementsManagement.AdTypeManagement.Queries;
using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementTypeManagementController(IMediator _mediator) : BaseController
    {
        [HttpGet]
        [Route("ad-types-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetAdTypesDDl()
        {
            try
            {
                return await _mediator.Send(new AdTypesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdTypesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
