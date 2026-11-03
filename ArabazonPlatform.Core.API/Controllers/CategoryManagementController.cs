using ArabazonPlatform.Generic.Generic;
using Azure.Core;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DashBoardManagement.CategoriesManagement.Commands;
using ArabazonPlatform.Application.Services.DashBoardManagement.CategoriesManagement.Queries;
using ArabazonPlatform.Generic.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArabazonPlatform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryManagementController(IMediator _mediator) : BaseController
    {
        [HttpGet]
        [Route("categories")]
        public async Task<ActionResult<BaseResponse<List<TreeNode<CategoryModel>>>>> GetCategories(int pageNumber, int pageSize, string? searchText = null)
        {
            try
            {
                BaseResponse<List<CategoryModel>> categoryModelsResponse = await _mediator.Send(new CategoriesQuery.Query(pageNumber, pageSize, searchText));
                if (!categoryModelsResponse.IsSuccess)
                {
                    return new BaseResponse<List<TreeNode<CategoryModel>>>(false, categoryModelsResponse.Message, categoryModelsResponse.ErrorCode);
                }
                List<TreeNode<CategoryModel>> treeCategoryNodes = new List<TreeNode<CategoryModel>>();
                foreach (CategoryModel categoryModel in categoryModelsResponse.DataObject.Where(p => string.IsNullOrEmpty(searchText) ? !p.ParentCategoryId.HasValue : true))
                {
                    TreeNode<CategoryModel> treeNode = new TreeNode<CategoryModel>();
                    treeNode.Data = categoryModel;
                    treeNode.Parent = categoryModel;
                    treeNode.Children = GetTreeNodes(categoryModel, categoryModelsResponse.DataObject);
                    treeCategoryNodes.Add(treeNode);
                }
                return new BaseResponse<List<TreeNode<CategoryModel>>>(true,string.Empty, treeCategoryNodes.Skip(pageNumber * pageSize).Take(pageSize).ToList());
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetCategories), UserAccessor.GetCurrentUserId(), pageNumber, pageSize, searchText);
                return new BaseResponse<List<TreeNode<CategoryModel>>>(false, ex.Message);
            }
        }

        private List<TreeNode<CategoryModel>> GetTreeNodes(CategoryModel categoryModel, List<CategoryModel> categoryModels)
        {
            List<TreeNode<CategoryModel>> treeNodes = new List<TreeNode<CategoryModel>>();
            categoryModel.ChildCategories = categoryModels.Where(p => p.ParentCategoryId == categoryModel.CategoryId).OrderBy(p => p.CategoryId).ToList();
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

        [HttpGet]
        [Route("categories-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetCategoriesCount(string? searchText = null)
        {
            try
            {
                BaseResponse<List<CategoryModel>> categoriesResponse = await _mediator.Send(new CategoriesQuery.Query(0, int.MaxValue, searchText,true));
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

        [HttpGet]
        [Route("categories-ddl")]
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

        [HttpGet]
        [Route("parent-categories-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetParentCategoriesDDl()
        {
            try
            {
                return await _mediator.Send(new CategoriesDDLQuery.Query(null, true, false, false, null, null));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetParentCategoriesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

        [HttpGet]
        [Route("child-categories-ddl")]
        public async Task<ActionResult<BaseResponse<List<DDLDataSource>>>> GetChildCategoriesDDl()
        {
            try
            {
                return await _mediator.Send(new CategoriesDDLQuery.Query(null, false, true, false, null, null));
            }
            catch (Exception ex)
            {
                await ExceptionService.LogErrorAsync(ex, nameof(GetChildCategoriesDDl), UserAccessor.GetCurrentUserId());
                return new BaseResponse<List<DDLDataSource>>(false, ex.Message);
            }
        }

    }
}
