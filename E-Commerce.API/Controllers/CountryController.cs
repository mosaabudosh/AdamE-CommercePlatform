using Adam.Generic.Generic;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services;
using E_Commerce.Application.Services.Country.Queries;
using E_Commerce.Application.Services.UserType.Queries;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController(IMediator _mediator) : BaseController
    {
        
        [Route("countries")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCountries()
        {
            try
            {
                return await _mediator.Send(new CountryQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCountries), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
