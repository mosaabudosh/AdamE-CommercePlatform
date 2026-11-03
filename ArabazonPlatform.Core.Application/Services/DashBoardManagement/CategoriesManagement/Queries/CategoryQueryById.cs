using ArabazonPlatform.Generic.Generic;
using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.DashBoard;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Domains.DashBoard;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.DashBoardManagement.CategoriesManagement.Queries
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
