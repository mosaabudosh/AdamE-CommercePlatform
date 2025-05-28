using Adam.Generic.Generic;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services.City.Commands;
using E_Commerce.Application.Services.City.Queries;
using E_Commerce.Domain.Domains;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityManagementController(IMediator _mediator) : BaseController
    {
        [Route("cities")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CityModel>>>> GetCities(int pageNumber, int pageSize,string? searchText = null, int? countryId = null)
        {
            try
            {
                return await _mediator.Send(new CitiesQuery.Query(pageNumber, pageSize, searchText, countryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCities), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, countryId);
                return new BaseResponse<List<CityModel>>(false, ex.Message);
            }
        }

        [Route("cities-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetCitiesCount(string? searchText = null, int? countryId = null)
        {
            try
            {
                BaseResponse<List<CityModel>> citiesResponse = await _mediator.Send(new CitiesQuery.Query(0, int.MaxValue, searchText, countryId));
                if (!citiesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, citiesResponse.Message, citiesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, citiesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCitiesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("city-by-id/{cityId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CityModel>>> CityById(int cityId)
        {
            try
            {
                return await _mediator.Send(new CityQueryById.Query(cityId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CityById), UserAccessor.GetCurrentUserId());
                return new BaseResponse<CityModel>(false, ex.Message);
            }
        }

        [Route("cities-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCitiesDDl()
        {
            try
            {
                return await _mediator.Send(new CitiesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCitiesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("city-creation")]
        public async Task<ActionResult<BaseResponse>> CreateCity([FromBody] CityModel cityModel)
        {
            try
            {
                return await _mediator.Send(new CityCreationCommands.Command(cityModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCity), UserAccessor.GetCurrentUserId(), cityModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("city-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateCity([FromBody] CityModel cityModel)
        {
            try
            {
                return await _mediator.Send(new CityUpdationCommands.Command(cityModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCity), UserAccessor.GetCurrentUserId(), cityModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("city-deletion/{cityId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCity(int cityId)
        {
            try
            {
                return await _mediator.Send(new CityDeletionCommands.Command(cityId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCity), UserAccessor.GetCurrentUserId(), cityId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
