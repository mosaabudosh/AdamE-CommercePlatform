using ArabazonPlatform.Application.Model.Advertisement;
using ArabazonPlatform.Application.Services.AdvertisementsManagement.AdPackageManagement.Commands;
using ArabazonPlatform.Application.Services.AdvertisementsManagement.AdPackageManagement.Queries;
using ArabazonPlatform.Application.Services.AdvertisementsManagement.AdvertisementManagement.Commands;
using ArabazonPlatform.Application.Services.AdvertisementsManagement.AdvertisementManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementManagementController(IMediator _mediator) : BaseController
    {
        [HttpPost]
        [Route("ad-creation")]
        public async Task<ActionResult<BaseResponse>> AddAdvertisementModel([FromBody] AdvertisementModel advertisementModel)
        {
            try
            {
                return await _mediator.Send(new AdvertisementCreationCommands.Command(advertisementModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(AddAdvertisementModel), UserAccessor.GetCurrentUserId(), advertisementModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("ad-modification")]
        public async Task<ActionResult<BaseResponse>> UpdateAdvertisement([FromBody] AdvertisementModel advertisementModel)
        {
            try
            {
                return await _mediator.Send(new AdvertisementUpdationCommands.Command(advertisementModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdateAdvertisement), UserAccessor.GetCurrentUserId(), advertisementModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ad-by-id/{advertisementId}")]
        public async Task<ActionResult<BaseResponse<AdvertisementModel>>> GetAdvertisementById(int advertisementId)
        {
            try
            {
                return await _mediator.Send(new AdvertisementQueryById.Query(advertisementId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdvertisementById), UserAccessor.GetCurrentUserId(), advertisementId);
                return new BaseResponse<AdvertisementModel>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("ad-deletion/{advertisementId}")]
        public async Task<ActionResult<BaseResponse>> DeleteAdvertisement(int advertisementId)
        {
            try
            {
                return await _mediator.Send(new AdvertisementDeletionCommands.Command(advertisementId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteAdvertisement), UserAccessor.GetCurrentUserId(), advertisementId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ads")]
        public async Task<ActionResult<BaseResponse<List<AdvertisementModel>>>> GetAdvertisements(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new AdvertisementsQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdvertisements), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<AdvertisementModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("ads-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetAdvertisementsCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<AdvertisementModel>> responseModels = await _mediator.Send(new AdvertisementsQuery.Query(0, int.MaxValue, searchText));
                if (!responseModels.IsSuccess)
                {
                    return new BaseResponse<int>(false, responseModels.Message, responseModels.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, responseModels.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetAdvertisementsCount), UserAccessor.GetCurrentUserId(), searchText);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

    }
}
