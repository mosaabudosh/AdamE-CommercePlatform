using Adam.Generic.Generic;
using E_Commerce.Application.Interfaces;
using E_Commerce.Application.Model;
using E_Commerce.Application.Services;
using E_Commerce.Application.Services.Country.Commands;
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
    public class CountryManagementController(IMediator _mediator) : BaseController
    {

        [Route("countries")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CountryModel>>>> GetCountries(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new CountriesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCountries), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<CountryModel>>(false, ex.Message);
            }
        }

        [Route("countries-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetCountriesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<CountryModel>> countriesResponse = await _mediator.Send(new CountriesQuery.Query(0, int.MaxValue, searchText));
                if (!countriesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, countriesResponse.Message, countriesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, countriesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCountriesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("country-by-id/{countryId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CountryModel>>> CountryById(int countryId)
        {
            try
            {
                return await _mediator.Send(new CountryQueryById.Query(countryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CountryById), UserAccessor.GetCurrentUserId());
                return new BaseResponse<CountryModel>(false, ex.Message);
            }
        }

        [Route("countries-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCountries()
        {
            try
            {
                return await _mediator.Send(new CountriesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCountries), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("country-creation")]
        public async Task<ActionResult<BaseResponse>> CreateCountry([FromBody] CountryModel countryModel)
        {
            try
            {
                return await _mediator.Send(new CountryCreationCommands.Command(countryModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCountry), UserAccessor.GetCurrentUserId(), countryModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("country-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateCountry([FromBody] CountryModel countryModel)
        {
            try
            {
                return await _mediator.Send(new CountryUpdationCommands.Command(countryModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateCountry), UserAccessor.GetCurrentUserId(), countryModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("country-deletion/{countryId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCountry(int countryId)
        {
            try
            {
                return await _mediator.Send(new CountryDeletionCommands.Command(countryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCountry), UserAccessor.GetCurrentUserId(), countryId);
                return new BaseResponse(false, ex.Message);
            }
        }

    }
}
