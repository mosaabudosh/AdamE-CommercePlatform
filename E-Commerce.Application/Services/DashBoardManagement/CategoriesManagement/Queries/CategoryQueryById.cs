using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Caching;
using E_Commerce.Application.Model.DashBoard;
using E_Commerce.Application.Services.DMSManagement.FolderManagement.Queries;
using E_Commerce.Domain.Domains.DashBoard;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using MediatR;
using static E_Commerce.Domain.Enum.Enums;

namespace E_Commerce.Application.Services.DashBoardManagement.CategoriesManagement.Queries
{
    public class CategoryQueryById
    {
        public class Query : IRequest<BaseResponse<CategoryModel>>
        {
            public int CategoryId { get; set; }
            public Query(int categoryId)
            {
                CategoryId = categoryId;
            }
        }

        public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService, CachingPlatformSettings _cachingPlatformSettings, CachingDataFromDMS _cachingDataFromDMS) : IRequestHandler<Query, BaseResponse<CategoryModel>>
        {
            public async Task<BaseResponse<CategoryModel>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    CategoryModel category = _unitOfWork.GetRepository<Category>().GetByExpressionAsNoTracking(p => p.CategoryId == request.CategoryId).Select(s => new CategoryModel()
                    {
                        CategoryId = s.CategoryId,
                        Name = s.Name,
                        NameAr = s.NameAr,
                        Code = s.Code,
                        Descreption = s.Descreption,
                        Image = s.Image,
                        IsBlock = s.IsBlock,
                        ParentCategoryId = s.ParentCategoryId,
                        GenderId = s.GenderId,
                        IsViewInHome = s.IsViewInHome,
                        IsViewInFooter = s.IsViewInFooter,
                        IsKids = s.IsKids,
                        IsBeauty = s.IsBeauty
                    }).FirstOrDefault();

                    if (!string.IsNullOrEmpty(category.Image))
                    {
                        List<FileCreationModel> responseGetFileCreationModels = _cachingDataFromDMS[PlatformSettingsCode.DMSCategoryPath, $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath]}\\{category.CategoryId}"];
                        if (responseGetFileCreationModels.Any())
                        {
                            category.FileCreationModel = responseGetFileCreationModels.FirstOrDefault();
                        }
                    }

                    return new BaseResponse<CategoryModel>(true, string.Empty, string.Empty, category);
                }
                catch (Exception ex)
                {
                    while (ex.InnerException is not null)
                    {
                        ex = ex.InnerException;
                    }
                    await _exceptionService.LogErrorAsync(ex, "CategoryQueryById", _userAccessor.GetCurrentUserId());
                    return new BaseResponse<CategoryModel>(false, ex.Message);
                }
            }
        }
    }
}
