using Adam.Generic.Generic;
using Adam.Logging.Services.MSSQLServer;
using E_Commerce.Application.Services.AdvertisementsManagement.AdTypeManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using E_Commerce.Infrastructure.HttpServices;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
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
