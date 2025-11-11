using Adam.Generic.Generic;
using Azure.Core;
using E_Commerce.Application.Model.CMS;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Model.Settings;
using E_Commerce.Application.Model.Store;
using E_Commerce.Application.Services.CMSManagement.StaticPageManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CitiesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.CountriesManagement.Queries;
using E_Commerce.Application.Services.DashBoardManagement.Dashboard.Queries;
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
using System.Collections.Generic;

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
        [HttpGet]
        [Route("dashboard-filters")]
        public async Task<ActionResult<BaseResponse<DashboardFiltersModel>>> GetDashboardFilters(string? searchText = null, string? language = null, int? genderId = null)
        {
            try
            {
                return await _mediator.Send(new DashboardFiltersQuery.Query(searchText, language, genderId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDashboardFilters), UserAccessor.GetCurrentUserId(), searchText, language, genderId);
                return new BaseResponse<DashboardFiltersModel>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("dashboard-categories")]
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
        [HttpGet]
        [Route("dashboard-shops")]
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
        [HttpGet]
        [Route("dashboard-featured-products")]
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
        [HttpGet]
        [Route("store-dashboard-products")]
        public async Task<ActionResult<BaseResponse<List<DashboardProductModel>>>> GetStoreDashboardProducts(int? pageNumber, int? pageSize, string? searchText = null, string? categoryIds = null, string? shopIds = null, string? brandIds = null, bool? isLoadImage = true,bool? isNewArrivals = false, decimal? fromPrice = null,decimal? toPrice = null)
        {
            try
            {
                return await _mediator.Send(new StoreProductsQuery.Query(pageNumber.HasValue ? pageNumber.Value : 0, pageSize.HasValue ? pageSize.Value : int.MaxValue, searchText, categoryIds, shopIds, brandIds, isLoadImage, isNewArrivals, fromPrice, toPrice));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetStoreDashboardProducts), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText, categoryIds, shopIds, isLoadImage, fromPrice, toPrice);
                return new BaseResponse<List<DashboardProductModel>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("dashboard-payment-methods")]
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
        [HttpGet]
        [Route("parent-categories-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetParentCategoriesDDl(bool IsJustViewInHome = false, int? genderId = null)
        {
            try
            {
                BaseResponse<List<CategoryModel>> categoryModelsResponse = await _mediator.Send(new CategoriesQuery.Query(0, int.MaxValue, null,false,false,null,false));
                if (!categoryModelsResponse.IsSuccess)
                {
                    return new BaseResponse<List<DDLDataSource>>(false, categoryModelsResponse.Message, categoryModelsResponse.ErrorCode);
                }

                List<DDLDataSource> parentCategoriesDDls = new List<DDLDataSource>();
                foreach (CategoryModel categoryModel in categoryModelsResponse.DataObject.Where(p => !p.ParentCategoryId.HasValue && (IsJustViewInHome ? p.IsViewInHome : true) && (!genderId.HasValue || !p.GenderId.HasValue || p.GenderId == genderId)))
                {
                    parentCategoriesDDls.Add(new DDLDataSource()
                    {
                        Label = categoryModel.Name,
                        Value = categoryModel.CategoryId,
                        Attributes = new
                        {
                            ParentCategoryId = categoryModel.ParentCategoryId,
                            Childs = GetTreeNodes(categoryModel, categoryModelsResponse.DataObject, IsJustViewInHome, genderId)
                        }
                    });
                }
                return new BaseResponse<List<DDLDataSource>>(true,string.Empty, parentCategoriesDDls);
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetParentCategoriesDDl), UserAccessor.GetCurrentUserId(), genderId);
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        private List<TreeNode<CategoryModel>> GetTreeNodes(CategoryModel categoryModel, List<CategoryModel> categoryModels, bool IsJustViewInHome = false, int? genderId = null)
        {
            List<TreeNode<CategoryModel>> treeNodes = new List<TreeNode<CategoryModel>>();
            categoryModel.ChildCategories = categoryModels.Where(p => p.ParentCategoryId == categoryModel.CategoryId && (IsJustViewInHome ? p.IsViewInHome : true) && (!genderId.HasValue || !p.GenderId.HasValue || p.GenderId == genderId)).OrderBy(p => p.CategoryId).ToList();
            foreach (CategoryModel child in categoryModel.ChildCategories)
            {
                TreeNode<CategoryModel> treeNode = new TreeNode<CategoryModel>();
                child.ChildCategories = categoryModels.Where(p => p.ParentCategoryId == child.CategoryId).OrderBy(p => p.CategoryId).ToList();
                treeNode.Data = child;
                treeNode.Parent = categoryModel;
                treeNode.Children = child.ChildCategories != null && child.ChildCategories.Count > 0 ? GetTreeNodes(child, categoryModels) : null;
                treeNodes.Add(treeNode);
            }
            return treeNodes;
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("child-categories-ddl")]
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
        [HttpGet]
        [Route("countries-ddl")]
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
        [HttpGet]
        [Route("settings-by-group")]
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

        [AllowAnonymous]
        [HttpGet]
        [Route("dashboard-footer")]
        public async Task<ActionResult<BaseResponse<DashboardFooterModel>>> GetDashboardFooters(string? language = null)
        {
            try
            {
                return await _mediator.Send(new DashboardFootersQuery.Query(language));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetDashboardFooters), UserAccessor.GetCurrentUserId(), language);
                return new BaseResponse<DashboardFooterModel>(false, ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("static-page-by-id/{staticPageId}")]
        public async Task<ActionResult<BaseResponse<StaticPageModel>>> StaticPageById(int staticPageId)
        {
            try
            {
                return await _mediator.Send(new StaticPageQueryById.Query(staticPageId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(StaticPageById), UserAccessor.GetCurrentUserId(), staticPageId);
                return new BaseResponse<StaticPageModel>(false, ex.Message);
            }
        }
    }
}
