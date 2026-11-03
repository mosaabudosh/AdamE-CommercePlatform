using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using ArabazonPlatform.Domain.Domains;
using ArabazonPlatform.API.Model.ReportModel;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.ItemsManagement.Queries;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandManagementController(IMediator _mediator) : BaseController
    {
        [Route("brands")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<BrandModel>>>> GetBrands(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new BrandsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetBrands), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<BrandModel>>(false, ex.Message);
            }
        }

        [Route("brands-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetBrandsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<BrandModel>> BrandsResponse = await _mediator.Send(new BrandsQuery.Query(0, int.MaxValue, searchText));
                if (!BrandsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, BrandsResponse.Message, BrandsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, BrandsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetBrandsCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("brand-by-id/{brandId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<BrandModel>>> BrandById(int brandId)
        {
            try
            {
                return await _mediator.Send(new BrandQueryById.Query(brandId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(BrandById), UserAccessor.GetCurrentUserId(), brandId);
                return new BaseResponse<BrandModel>(false, ex.Message);
            }
        }

        [Route("brands-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetBrandsDDl()
        {
            try
            {
                return await _mediator.Send(new BrandsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetBrandsDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("brand-creation")]
        public async Task<ActionResult<BaseResponse>> CreateBrand([FromBody] BrandModel brandModel)
        {
            try
            {
                return await _mediator.Send(new BrandCreationCommands.Command(brandModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateBrand), UserAccessor.GetCurrentUserId(), brandModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("brand-modification")]
        public async Task<ActionResult<BaseResponse>> UpdatBrand([FromBody] BrandModel brandModel)
        {
            try
            {
                return await _mediator.Send(new BrandUpdationCommands.Command(brandModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdatBrand), UserAccessor.GetCurrentUserId(), brandModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("brand-deletion/{brandId}")]
        public async Task<ActionResult<BaseResponse>> DeleteBrand(int brandId)
        {
            try
            {
                return await _mediator.Send(new BrandDeletionCommands.Command(brandId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteBrand), UserAccessor.GetCurrentUserId(), brandId);
                return new BaseResponse(false, ex.Message);
            }
        }

    }
}
