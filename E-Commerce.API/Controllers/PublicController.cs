using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CitiesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.PaymentMethodsManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.ShopsManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.StoreDashboard.Queries;
using E_Commerce.Application.Services.Gender.Queries;
using E_Commerce.Application.Services.SettingsManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController(IMediator _mediator) : BaseController
    {
        [AllowAnonymous]
        [HttpGet]
        [Route("genders-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetGenders()
        {
            try
            {
                return await _mediator.Send(new GendersDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetGenders), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("cities-ddl")]
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

        [AllowAnonymous]
        [Route("dashboard-categories")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CategoryModel>>>> GetDashboardCategories(int? pageNumber, int? pageSize, string? searchText = null, int? parentCategoryId = null)
        {
            try
            {
                return await _mediator.Send(new CategoriesQuery.Query(pageNumber.HasValue ? pageNumber.Value : 0, pageSize.HasValue ? pageSize.Value : int.MaxValue, searchText, false, true, parentCategoryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDashboardCategories), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, parentCategoryId);
                return new BaseResponse<List<CategoryModel>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("dashboard-shops")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<ShopModel>>>> GetShops(int? pageNumber, int? pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new ShopsQuery.Query(pageNumber.HasValue ? pageNumber.Value : 0, pageSize.HasValue ? pageSize.Value : int.MaxValue, searchText, null, null, true));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetShops), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<ShopModel>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("dashboard-featured-products")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DashboardProductModel>>>> GetDashboardFeaturedProducts(int? pageNumber, int? pageSize,string? searchText = null, string? categoryIds = null, string? shopIds = null, bool? isLoadImage = true, decimal? fromPrice = null, decimal? toPrice = null)
        {
            try
            {
                return await _mediator.Send(new FeaturedProductsQuery.Query(pageNumber.HasValue ? pageNumber.Value : 0, pageSize.HasValue ? pageSize.Value : int.MaxValue, searchText, categoryIds, shopIds, isLoadImage, fromPrice, toPrice));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDashboardFeaturedProducts), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, categoryIds, shopIds, isLoadImage, fromPrice, toPrice);
                return new BaseResponse<List<DashboardProductModel>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("store-dashboard-products")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DashboardProductModel>>>> GetStoreDashboardProducts(int? pageNumber, int? pageSize, string? searchText = null, string? categoryIds = null, string? shopIds = null, bool? isLoadImage = true,decimal? fromPrice = null,decimal? toPrice = null)
        {
            try
            {
                return await _mediator.Send(new StoreProductsQuery.Query(pageNumber.HasValue ? pageNumber.Value : 0, pageSize.HasValue ? pageSize.Value : int.MaxValue, searchText, categoryIds, shopIds, isLoadImage, fromPrice, toPrice));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetStoreDashboardProducts), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, categoryIds, shopIds, isLoadImage, fromPrice, toPrice);
                return new BaseResponse<List<DashboardProductModel>>(false, ex.Message);
            }
        }

        [Route("dashboard-payment-methods")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<PaymentMethodModel>>>> GetPaymentMethods()
        {
            try
            {
                return await _mediator.Send(new PaymentMethodsQuery.Query(0,int.MaxValue));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetPaymentMethods), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<PaymentMethodModel>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("parent-categories-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetParentCategoriesDDl(bool IsJustViewInHome = false, int? genderId = null)
        {
            try
            {
                return await _mediator.Send(new CategoriesDDLQuery.Query(null, true, false, IsJustViewInHome, null, genderId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetParentCategoriesDDl), UserAccessor.GetCurrentUserId(), genderId);
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [Route("child-categories-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetChildCategoriesDDl(bool IsJustViewInHome = false, int? parentCategoryId = null, int? genderId = null)
        {
            try
            {
                return await _mediator.Send(new CategoriesDDLQuery.Query(null, false, true, IsJustViewInHome, parentCategoryId, genderId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetChildCategoriesDDl), UserAccessor.GetCurrentUserId(), parentCategoryId, genderId);
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [Route("settings-by-group")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<SettingsModel>>>> GetSettingsByGroup(string groupName)
        {
            try
            {
                return await _mediator.Send(new SettingsByGroupQuery.Query(groupName));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetSettingsByGroup), UserAccessor.GetCurrentUserId(), groupName);
                return new BaseResponse<List<SettingsModel>>(false, ex.Message);
            }
        }
    }
}
