using Adam.Generic.Generic;
using E_Commerce.Application.Model.Advertisement;
using E_Commerce.Application.Services.AdvertisementsManagement.AdPackageManagement.Commands;
using E_Commerce.Application.Services.AdvertisementsManagement.AdPackageManagement.Queries;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Commands;
using E_Commerce.Application.Services.AuthenticationManagement.ActionsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementPackageManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("ad-package-creation")]
        public async Task<ActionResult<BaseResponse>> AddAdPackage([FromBody] AdPackageModel adPackageModel)
        {
            try
            {
                return await _mediator.Send(new AdPackageCreationCommands.Command(adPackageModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AddAdPackage), UserAccessor.GetCurrentUserId(), adPackageModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("ad-package-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateAdPackage([FromBody] AdPackageModel adPackageModel)
        {
            try
            {
                return await _mediator.Send(new AdPackageUpdationCommands.Command(adPackageModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateAdPackage), UserAccessor.GetCurrentUserId(), adPackageModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ad-package-by-id/{adPackageId}")]
        public async Task<ActionResult<BaseResponse<AdPackageModel>>> GetAdPackageById(int adPackageId)
        {
            try
            {
                return await _mediator.Send(new AdPackageQueryById.Query(adPackageId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdPackageById), UserAccessor.GetCurrentUserId(), adPackageId);
                return new BaseResponse<AdPackageModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("ad-package-deletion/{adPackageId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAdPackage(int adPackageId)
        {
            try
            {
                return await _mediator.Send(new AdPackageDeletionCommands.Command(adPackageId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteAdPackage), UserAccessor.GetCurrentUserId(), adPackageId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ad-packages")]
        public async Task<ActionResult<BaseResponse<List<AdPackageModel>>>> GetAdPackages(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new AdPackagesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdPackages), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<AdPackageModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ad-packages-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetAdPackagesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<AdPackageModel>> responseActionModels = await _mediator.Send(new AdPackagesQuery.Query(0, int.MaxValue, searchText));
                if (!responseActionModels.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseActionModels.Message, responseActionModels.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, responseActionModels.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdPackagesCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ad-packages-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetAdPackagesDDl()
        {
            try
            {
                return await _mediator.Send(new AdPackagesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdPackagesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }
    }
}
