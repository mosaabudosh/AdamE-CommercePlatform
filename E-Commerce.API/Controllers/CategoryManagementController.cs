using Adam.Generic.Generic;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Commands;
using E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries;
using Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryManagementController(IMediator _mediator) : BaseController
    {
        [Route("categories")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<CategoryModel>>>> GetCategories(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                return await _mediator.Send(new CategoriesQuery.Query(pageNumber, pageSize, searchText));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCategories), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<CategoryModel>>(false, ex.Message);
            }
        }

        [Route("categories-count")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<int>>> GetCategoriesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<CategoryModel>> categoriesResponse = await _mediator.Send(new CategoriesQuery.Query(0, int.MaxValue, searchText));
                if (!categoriesResponse.IsSuccess)
                {
                    return new BaseResponse<int>(false, categoriesResponse.Message, categoriesResponse.ErrorCode);
                }
                return new BaseResponse<int>(true, string.Empty, categoriesResponse.DataObject.Count());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCategoriesCount), UserAccessor.GetCurrentUserId());
                return new BaseResponse<int>(false, ex.Message);
            }
        }

        [Route("category-by-id/{categoryId}")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<CategoryModel>>> CategoryById(int categoryId)
        {
            try
            {
                return await _mediator.Send(new CategoryQueryById.Query(categoryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CategoryById), UserAccessor.GetCurrentUserId());
                return new BaseResponse<CategoryModel>(false, ex.Message);
            }
        }

        [Route("categories-ddl")]
        [HttpGet]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetCategoriesDDl()
        {
            try
            {
                return await _mediator.Send(new CategoriesDDLQuery.Query());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCategoriesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("category-creation")]
        public async Task<ActionResult<BaseResponse>> CreateCategory([FromBody] CategoryModel categoryModel)
        {
            try
            {
                return await _mediator.Send(new CategoryCreationCommands.Command(categoryModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(CreateCategory), UserAccessor.GetCurrentUserId(), categoryModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("category-modification")]
        public async Task<ActionResult<BaseResponse>> UpdatCategory([FromBody] CategoryModel categoryModel)
        {
            try
            {
                return await _mediator.Send(new CategoryUpdationCommands.Command(categoryModel));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(UpdatCategory), UserAccessor.GetCurrentUserId(), categoryModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        [HttpPost]
        [Route("category-deletion/{categoryId}")]
        public async Task<ActionResult<BaseResponse>> DeleteCategory(int categoryId)
        {
            try
            {
                return await _mediator.Send(new CategoryDeletionCommands.Command(categoryId));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(DeleteCategory), UserAccessor.GetCurrentUserId(), categoryId);
                return new BaseResponse(false, ex.Message);
            }
        }
    }
}
