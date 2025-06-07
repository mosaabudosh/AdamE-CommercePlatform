using Adam.Generic.Generic;
using E_Commerce.API.Model.ReportModel;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Services.DashBoardManagement.ShopsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ShopsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static QRCoder.PayloadGenerator;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopManagementController(IMediator _mediator) : BaseController
    {
        [Route("shops")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopModel>>>> GetShops(int pageNumber, int pageSize, string? searchText = null,int? cityId = null, int? parentShopId = null)
        {
            try
            {
                return await _mediator.Send(new ShopsQuery.Query(pageNumber, pageSize, searchText, cityId, parentShopId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShops), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, cityId,parentShopId);
                return new BaseResponse<List<ShopModel>>(false, ex.Message);
            }
        }

        [Route("shops-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetShopsCount(string? searchText = null, int? cityId = null, int? parentShopId = null)
        {
            try
            {
                BaseResponse<List<ShopModel>> shopsResponse = await _mediator.Send(new ShopsQuery.Query(0, int.MaxValue, searchText, cityId, parentShopId));
                if (!shopsResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, shopsResponse.Message, shopsResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, shopsResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopsCount), UserAccessor.GetCurrentUserId(), searchText, cityId,parentShopId);
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("shop-by-id/{shopId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<ShopModel>>> ShopById(int shopId)
        {
            try
            {
                return await _mediator.Send(new ShopQueryById.Query(shopId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(ShopById), UserAccessor.GetCurrentUserId(), shopId);
                return new BaseResponse<ShopModel>(false, ex.Message);
            }
        }

        [Route("shops-dll")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetShopsDDL()
        {
            try
            {
                return await _mediator.Send(new ShopsDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShopsDDL), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-creation")]
        public async Task<ActionResult<BaseResponse>> CreateShop([FromBody] ShopModel shopModel)
        {
            try
            {
                return await _mediator.Send(new ShopCreationCommands.Command(shopModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateShop), UserAccessor.GetCurrentUserId(), shopModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-modification")]
        public async Task<ActionResult<BaseResponse>> UpdatShop([FromBody] ShopModel shopModel)
        {
            try
            {
                return await _mediator.Send(new ShopUpdationCommands.Command(shopModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdatShop), UserAccessor.GetCurrentUserId(), shopModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("shop-deletion/{shopId}")]
        public async Task<ActionResult<BaseResponse>> DeleteShop(int shopId)
        {
            try
            {
                return await _mediator.Send(new ShopDeletionCommands.Command(shopId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteShop), UserAccessor.GetCurrentUserId(), shopId);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("shops-printing")]
        public async Task<IActionResult> ShopsPrinting(string? searchText, int? cityId = null, int? parentShopId = null)
        {
            try
            {
                BaseResponse<List<ShopModel>> shopsResponse = await _mediator.Send(new ShopsQuery.Query(0, int.MaxValue, searchText, cityId, parentShopId));
                if (!shopsResponse.IsSuccess)
                {
                    throw new Exception(shopsResponse.Message);
                }

                Dictionary<string, object> dataSources = new Dictionary<string, object>();
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                List<ShopReportModel> shopsModels = shopsResponse.DataObject.Select(x => new ShopReportModel()
                {
                    Name = $"{x.Name} - {x.CommercialName}",
                    Code = x.Code,
                    Adresse = x.Adresse,
                    PhoneNumber = x.PhoneNumber,
                    City = x.City
                }).ToList();

                PlatformInfoModel platformInfoModel = CachingPlatformSettings.GetPlatformInfo();
                platformInfoModel.PrintedBy = UserAccessor.GetCurrentUserName();
                dataSources.Add("PlatformInfoModel", new[] { platformInfoModel });
                dataSources.Add("ShopModel", shopsModels);

                return ToPDF("\\Reports\\Shops.rdlc", dataSources, parameters);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await ExceptionService.LogErrorAsync(ex, nameof(ShopsPrinting), UserAccessor.GetCurrentUserId(), searchText, cityId, parentShopId);
                return BadRequest(ex.Message);
            }
        }
    }
}
