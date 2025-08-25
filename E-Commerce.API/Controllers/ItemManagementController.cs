using Adam.Generic.Generic;
using E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.ItemsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using E_Commerce.Application.Model.Authorisation;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Services.AuthenticationManagement.UsersManagement.Queries;
using E_Commerce.Domain.Domains;
using E_Commerce.API.Model.ReportModel;
using E_Commerce.Application.Model.DashBoard;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemManagementController(IMediator _mediator) : BaseController
    {
        //[Route("items")]
        //[HttpGet]
        //public async Task<ActionResult<BaseResponse<List<ItemModel>>>> GetItems(int pageNumber, int pageSize, string? searchText = null,int? categoryId = null)
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemsQuery.Query(pageNumber, pageSize, searchText, categoryId));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(GetItems), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, categoryId);
        //        return new BaseResponse<List<ItemModel>>(false, ex.Message);
        //    }
        //}

        //[Route("items-count")]
        //[HttpGet]
        //public async Task<ActionResult<BaseResponse<int>>> GetItemsCount(string? searchText = null, int? categoryId = null)
        //{
        //    try
        //    {
        //        BaseResponse<List<ItemModel>> itemsResponse = await _mediator.Send(new ItemsQuery.Query(0, int.MaxValue, searchText, categoryId));
        //        if (!itemsResponse.IsSuccess)
        //        {
        //            return new BaseResponse<int>(false, itemsResponse.Message, itemsResponse.ErrorCode);
        //        }
        //        return new BaseResponse<int>(true, string.Empty, itemsResponse.DataObject.Count());
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(GetItemsCount), UserAccessor.GetCurrentUserId(),searchText, categoryId);
        //        return new BaseResponse<int>(false, ex.Message);
        //    }
        //}

        //[Route("item-by-id/{itemId}")]
        //[HttpGet]
        //public async Task<ActionResult<BaseResponse<ItemModel>>> ItemById(string itemId)
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemQueryById.Query(new Guid(itemId)));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(ItemById), UserAccessor.GetCurrentUserId(), itemId);
        //        return new BaseResponse<ItemModel>(false, ex.Message);
        //    }
        //}

        //[Route("items-ddl")]
        //[HttpGet]
        //public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetItemsDDl()
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemsDDLQuery.Query());
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(GetItemsDDl), UserAccessor.GetCurrentUserId());
        //        return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("item-creation")]
        //public async Task<ActionResult<BaseResponse>> CreateItem([FromBody] ItemModel itemModel)
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemCreationCommands.Command(itemModel));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(CreateItem), UserAccessor.GetCurrentUserId(), itemModel);
        //        return new BaseResponse(false, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("item-modification")]
        //public async Task<ActionResult<BaseResponse>> UpdatItem([FromBody] ItemModel itemModel)
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemUpdationCommands.Command(itemModel));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(UpdatItem), UserAccessor.GetCurrentUserId(), itemModel);
        //        return new BaseResponse(false, ex.Message);
        //    }
        //}

        //[HttpPost]
        //[Route("item-deletion/{itemId}")]
        //public async Task<ActionResult<BaseResponse>> DeleteItem(Guid itemId)
        //{
        //    try
        //    {
        //        return await _mediator.Send(new ItemDeletionCommands.Command(itemId));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ExceptionService.LogErrorAsync(ex, nameof(DeleteItem), UserAccessor.GetCurrentUserId(), itemId);
        //        return new BaseResponse(false, ex.Message);
        //    }
        //}

        //[HttpGet]
        //[Route("items-printing")]
        //public async Task<IActionResult> ItemsPrinting(string? searchText, int? categoryId)
        //{
        //    try
        //    {
        //        BaseResponse<List<ItemModel>> itemsResponse = await _mediator.Send(new ItemsQuery.Query(0, int.MaxValue, searchText, categoryId));
        //        if (!itemsResponse.IsSuccess)
        //        {
        //            throw new Exception(itemsResponse.Message);
        //        }

        //        Dictionary<string, object> dataSources = new Dictionary<string, object>();
        //        Dictionary<string, string> parameters = new Dictionary<string, string>();
        //        List<ItemReportModel> itemsModels = itemsResponse.DataObject.Select(x => new ItemReportModel()
        //        {
        //            Name = HttpAccessor.GetCurrentUserLanguage() == Adam.Generic.Enum.LanguageCode.enUS ? $"{x.Name}" : $"{x.NameAr}",
        //            Code = x.Code,
        //            Category = x.Category
        //        }).ToList();

        //        PlatformInfoModel platformInfoModel = CachingPlatformSettings.GetPlatformInfo();
        //        platformInfoModel.PrintedBy = UserAccessor.GetCurrentUserName();
        //        dataSources.Add("PlatformInfoModel", new[] { platformInfoModel });
        //        dataSources.Add("ItemModel", itemsModels);

        //        return ToPDF("\\Reports\\Items.rdlc", dataSources, parameters);
        //    }
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException is not null)
        //        {
        //            ex = ex.InnerException;
        //        }
        //        await ExceptionService.LogErrorAsync(ex, nameof(ItemsPrinting), UserAccessor.GetCurrentUserId(), searchText, categoryId);
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
